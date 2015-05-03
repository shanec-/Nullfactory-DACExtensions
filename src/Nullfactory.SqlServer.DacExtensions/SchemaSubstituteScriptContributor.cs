//----------------//-----------------------------------------------------------------------
// <copyright file="SchemaSubstituteScriptContributor.cs" company="Nullfactory">
//     Copyright (c) Nullfactory. All rights reserved.
// </copyright>
// <author>Shane Carvalho</author>
//-----------------------------------------------------------------------

namespace Nullfactory.SqlServer.DacExtensions
{
    using System.Diagnostics;
    using Microsoft.SqlServer.Dac.Deployment;
    using Microsoft.SqlServer.TransactSql.ScriptDom;

    [ExportDeploymentPlanModifier("Nullfactory.SqlServer.DacExtensions.SchemaSubstituteScriptContributor", "1.0.0.0")]
    public class SchemaSubstituteScriptContributor : DeploymentPlanModifier
    {
        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            DeploymentStep nextStep = context.PlanHandle.Head;

            BeginPreDeploymentScriptStep beforePreDeploy = null;
            while (nextStep != null)
            {
                DeploymentStep currentStep = nextStep;
                nextStep = currentStep.Next;

                if (currentStep is BeginPreDeploymentScriptStep)
                {
                    beforePreDeploy = (BeginPreDeploymentScriptStep)currentStep;
                    continue;
                }

                if (currentStep is BeginPostDeploymentScriptStep)
                {
                    continue;
                }

                if (currentStep is SqlPrintStep)
                {
                    continue;
                }


                if (beforePreDeploy == null)
                {
                    continue;
                }

                DeploymentScriptDomStep domStep = currentStep as DeploymentScriptDomStep;
                if (domStep == null)
                {
                    continue;
                }

                TSqlScript script = domStep.Script as TSqlScript;
                if (script == null)
                {
                    continue;
                }

                int batchCount = script.Batches.Count;

                for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
                {
                    var statements = script.Batches[batchIndex].Statements;
                    int statementCount = statements.Count;

                    for (int statementIndex = 0; statementIndex < statementCount; statementIndex++)
                    {
                        TSqlStatement sqlStatement = statements[statementIndex];

                        string statementScript = string.Empty;
                        domStep.ScriptGenerator.GenerateScript(sqlStatement, out statementScript);

                        Debug.WriteLine("Original: " + sqlStatement);

                        sqlStatement.Accept(new OverrideSchemaVisitor());

                        Debug.WriteLine("Modified: " + sqlStatement);
                    }
                }
            }
        }
    }
}
