using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;
using Rule = UiPath.Studio.Activities.Api.Analyzer.Rules.Rule;

namespace WorkflowAnalyzerCustomActivity
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowanAlyzerConfigurationService)
        {
            if (!workflowanAlyzerConfigurationService.HasFeature("WorkflowAnalyzerV4"))
                return;

            var forbiddenStringRule = new Rule<IActivityModel>("NotAllowedInVariables", "DE-USG-001", InspectVariableForString);
            forbiddenStringRule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Error;
            forbiddenStringRule.Parameters.Add("string_in_variable", new Parameter()
            {
                DefaultValue = "demo",
                Key = "string_in_variable",
                LocalizedDisplayName = "Illegal string",
            });

            workflowanAlyzerConfigurationService.AddRule<IActivityModel>(forbiddenStringRule);
        }

        private InspectionResult InspectVariableForString(IActivityModel activityToInspect, Rule configuredRule)
        {
            var configuredString = configuredRule.Parameters["string_in_variable"]?.Value;

            if (string.IsNullOrEmpty(configuredString))
            {
                return new InspectionResult() { HasErrors = false };
            }

            if (activityToInspect.Variables.Count == 0)
            {
                return new InspectionResult() { HasErrors = false };
            }

            var messageList = new List<InspectionMessage>();

            foreach (var variable in activityToInspect.Variables)
            {
                if (variable.DisplayName.Contains(configuredString))
                {
                    messageList.Add(new InspectionMessage()
                    {
                        Message = $"Variable {variable.DisplayName} contains an illegal string: {configuredString}"
                    });
                    
                }
            }

            if(messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "Fix your naming",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }

            return new InspectionResult() { HasErrors = false };
        }
    }
}
