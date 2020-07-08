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

        }
    }
}
