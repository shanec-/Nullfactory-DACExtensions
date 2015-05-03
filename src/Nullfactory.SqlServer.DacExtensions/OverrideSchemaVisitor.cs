//-----------------------------------------------------------------------
// <copyright file="OverrideSchemaVisitor.cs" company="Nullfactory">
//     Copyright (c) Nullfactory. All rights reserved.
// </copyright>
// <author>Shane Carvalho</author>
//-----------------------------------------------------------------------

namespace Nullfactory.SqlServer.DacExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SqlServer.TransactSql.ScriptDom;

    /// <summary>
    /// Override the Schema Identifier
    /// </summary>
    public class OverrideSchemaVisitor : TSqlFragmentVisitor
    {
        /// <summary>
        /// Explicit visit the foreign key constraint definition node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void ExplicitVisit(ForeignKeyConstraintDefinition node)
        {
            this.Visit(node);
        }

        /// <summary>
        /// Explicit visit the alter table statement node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void ExplicitVisit(AlterTableStatement node)
        {
            this.Visit(node);
        }

        /// <summary>
        /// Explicit visit the create table statement node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void ExplicitVisit(CreateTableStatement node)
        {
            this.Visit(node);
        }

        /// <summary>
        /// Visits the foreign key constraint node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Visit(ForeignKeyConstraintDefinition node)
        {
            node.ReferenceTableName.SchemaIdentifier.Value = "$TenantSchema";
            base.Visit(node);
        }

        /// <summary>
        /// Visits the alter table statement node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Visit(AlterTableStatement node)
        {
            node.SchemaObjectName.SchemaIdentifier.Value = "$TenantSchema";
            base.Visit(node);
        }

        /// <summary>
        /// Visits the create table statement node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Visit(CreateTableStatement node)
        {
            node.SchemaObjectName.SchemaIdentifier.Value = "$TenantSchema";
            base.Visit(node);
        }
    }
}
