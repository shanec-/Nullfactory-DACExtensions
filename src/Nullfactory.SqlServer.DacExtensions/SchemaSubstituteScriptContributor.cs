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

    /// <summary>
    /// Schema Substitute Script Contributor
    /// </summary>
    [ExportDeploymentPlanModifier("Nullfactory.SchemaSubstitute", "1.0.0.0")]
    public class SchemaSubstituteScriptContributor : DeploymentPlanModifier
    {
        /// <summary>
        /// Called by the deployment engine to allow custom contributors to execute their unique tasks
        /// </summary>
        /// <param name="context">A <see cref="T:Microsoft.SqlServer.Dac.Deployment.DeploymentContributorContext" /> object</param>
        protected override void OnExecute(DeploymentPlanContributorContext context)
        {
            // Initialize the command arguments
            string oldSchemaName = "dbo";
            string newSchemaName = "$TenantSchema";

            if (context.Arguments.ContainsKey("Nullfactory.SchemaSubstitute.OldSchemaName"))
            {
                context.Arguments.TryGetValue("Nullfactory.SchemaSubstitute.OldSchemaName", out oldSchemaName);
            }

            if (context.Arguments.ContainsKey("Nullfactory.SchemaSubstitute.NewSchemaName"))
            {
                context.Arguments.TryGetValue("Nullfactory.SchemaSubstitute.NewSchemaName", out newSchemaName);
            }

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

                        sqlStatement.Accept(new OverrideSchemaVisitor(oldSchemaName, newSchemaName));

                        Debug.WriteLine("Modified: " + sqlStatement);
                    }
                }
            }
        }
    }
}
