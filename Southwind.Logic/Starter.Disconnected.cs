﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Entities.Disconnected;
using Signum.Entities;
using Signum.Engine.Disconnected;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Entities.Chart;
using Signum.Entities.ControlPanel;
using Signum.Entities.Mailing;
using Southwind.Entities;
using Signum.Entities.UserQueries;
using Signum.Engine;
using System.Linq.Expressions;
using Signum.Utilities;
using System.Data.Common;
using Signum.Engine.Basics;
using Signum.Engine.Authorization;
using Signum.Engine.Operations;
using Signum.Entities.Files;
using Signum.Entities.Processes;
using Signum.Entities.Notes;
using Signum.Entities.Alerts;

namespace Southwind.Logic
{
    public partial class Starter
    {
        private static void SetupDisconnectedStrategies(SchemaBuilder sb)
        {
            //Signum.Entities
            DisconnectedLogic.Register<TypeDN>(Download.Replace, Upload.None);

            //Signum.Entities.Authorization
            DisconnectedLogic.Register<UserDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RoleDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<ResetPasswordRequestDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<RuleTypeDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RulePropertyDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RuleFacadeMethodDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RuleQueryDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RuleOperationDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<PermissionDN>(Download.Replace, Upload.None);
            DisconnectedLogic.Register<RulePermissionDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<SessionLogDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<UserTicketDN>(Download.None, Upload.None);

            //Signum.Entities.Basics
            DisconnectedLogic.Register<TypeConditionNameDN>(Download.Replace, Upload.None);
            DisconnectedLogic.Register<PropertyDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<FacadeMethodDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<QueryDN>(Download.All, Upload.New).Importer = new QueryImporter();
            
            //Signum.Entities.Notes
            DisconnectedLogic.Register<NoteDN>(Download.None, Upload.None);
            
            //Signum.Entities.Alerts
            DisconnectedLogic.Register<AlertTypeDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<AlertDN>(Download.None, Upload.None);

            //Signum.Entities.Chart
            DisconnectedLogic.Register<ChartColorDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<UserChartDN>(Download.All, Upload.New).Importer = new UserQueryImporter(); 
            DisconnectedLogic.Register<ChartScriptDN>(Download.All, Upload.New);

            //Signum.Entities.Files
            DisconnectedLogic.Register<FileDN>(Download.All, Upload.New);

            //Signum.Entities.ControlPanel
            DisconnectedLogic.Register<ControlPanelDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<UserChartPartDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<UserQueryPartDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<CountSearchControlPartDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<LinkListPartDN>(Download.None, Upload.None);

            //Signum.Entities.Disconnected
            DisconnectedLogic.Register<DisconnectedMachineDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<DisconnectedExportDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<DisconnectedImportDN>(Download.None, Upload.None);

            //Signum.Entities.Mailing
            DisconnectedLogic.Register<EmailMessageDN>(Download.None, Upload.None);
            DisconnectedLogic.Register<EmailTemplateDN>(Download.Replace, Upload.None);
            DisconnectedLogic.Register<EmailPackageDN>(Download.None, Upload.None);

            //Signum.Entities.Operations
            DisconnectedLogic.Register<OperationDN>(Download.Replace, Upload.None);
            Expression<Func<OperationLogDN, bool>> operationLogCondition = ol =>
             ol.Target.EntityType == typeof(EmployeeDN) ||
             ol.Target.EntityType == typeof(ProductDN) ||
             ol.Target.EntityType == typeof(OrderDN) && ((OrderDN)ol.Target.Entity).Employee.RefersTo(EmployeeDN.Current) ||
             ol.Target.EntityType == typeof(PersonDN) && Database.Query<OrderDN>().Any(o => o.Employee.RefersTo(EmployeeDN.Current) && o.Customer == ((PersonDN)ol.Target.Entity)) ||
             ol.Target.EntityType == typeof(CompanyDN) && Database.Query<OrderDN>().Any(o => o.Employee.RefersTo(EmployeeDN.Current) && o.Customer == ((CompanyDN)ol.Target.Entity));

            DisconnectedLogic.Register<OperationLogDN>(operationLogCondition, Upload.New);

            //Signum.Entities.Processes
            DisconnectedLogic.Register<ProcessDN>(Download.Replace, Upload.None);
            DisconnectedLogic.Register<ProcessExecutionDN>(Download.None, Upload.New);
            DisconnectedLogic.Register<UserProcessSessionDN>(Download.None, Upload.New);
            DisconnectedLogic.Register<PackageDN>(Download.None, Upload.New);
            DisconnectedLogic.Register<PackageOperationDN>(Download.None, Upload.New);
            DisconnectedLogic.Register<PackageLineDN>(Download.None, Upload.New);

            //Signum.Entities.Exceptions
            DisconnectedLogic.Register<ExceptionDN>(e => Database.Query<OperationLogDN>().Any(ol => operationLogCondition.Evaluate(ol) && ol.Exception.RefersTo(e)), Upload.New);
            
            //Signum.Entities.UserQueries
            DisconnectedLogic.Register<UserQueryDN>(Download.All, Upload.New).Importer = new UserQueryImporter();

            //Southwind.Entities
            DisconnectedLogic.Register<EmployeeDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<TerritoryDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<RegionDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<ProductDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<SupplierDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<CategoryDN>(Download.All, Upload.None);
            DisconnectedLogic.Register<PersonDN>(p => Database.Query<OrderDN>().Any(o => o.Employee.RefersTo(EmployeeDN.Current) && o.Customer == p), Upload.New);
            DisconnectedLogic.Register<CompanyDN>(p => Database.Query<OrderDN>().Any(o => o.Employee.RefersTo(EmployeeDN.Current) && o.Customer == p), Upload.New);
            DisconnectedLogic.Register<OrderDN>(o => o.Employee.RefersTo(EmployeeDN.Current));
            DisconnectedLogic.Register<ShipperDN>(Download.All, Upload.None);
        }
    }

    public class QueryImporter : BasicImporter<QueryDN>
    {
        protected override int Insert(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            var queries = Database.Query<QueryDN>().Select(a => a.Key).ToHashSet();
            List<QueryDN> newQueries = null;

            using (Connector.Override(newDatabase))
            {
                newQueries = Database.Query<QueryDN>()
                     .AsEnumerable()
                     .Where(a => !queries.Contains(a.Key)).ToList();
            }

            if (newQueries.Any())
            {
                foreach (var q in newQueries)
                {
                    q.SetId(null);
                    q.SetNew();
                }
                newQueries.SaveList();
            }

            return newQueries.Count;
        }
    }

    public class UserQueryImporter : BasicImporter<UserQueryDN>
    {
        protected override int Insert(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            var interval = GetNewIdsInterval(table, machine, newDatabase);

            if (interval == null)
                return 0;

            List<UserQueryDN> list;
            using (SqlConnector.Override(newDatabase))
            {
                list = Database.Query<UserQueryDN>().Where(a => interval.Value.Contains(a.Id)).ToList();
            }

            var queries = Database.Query<QueryDN>().ToDictionary(a => a.Key);


            foreach (var uq in list)
            {
                uq.SetNew();
                uq.Query = queries[uq.Query.Key];
            }

            using (DisconnectedTools.SaveAndRestoreNextId(table))
            {
                using (OperationLogic.AllowSave<UserQueryDN>())
                {
                    Administrator.SaveListDisableIdentity(list);
                }
            }

            return list.Count;
        }
    }

    public class UserChartImporter : BasicImporter<UserChartDN>
    {
        protected override int Insert(DisconnectedMachineDN machine, Table table, IDisconnectedStrategy strategy, SqlConnector newDatabase)
        {
            var interval = GetNewIdsInterval(table, machine, newDatabase);

            if (interval == null)
                return 0;

            List<UserChartDN> list;
            using (SqlConnector.Override(newDatabase))
            {
                list = Database.Query<UserChartDN>().Where(a => interval.Value.Contains(a.Id)).ToList();
            }

            var queries = Database.Query<QueryDN>().ToDictionary(a => a.Key);

            foreach (var uq in list)
            {
                uq.SetNew();
                uq.Query = queries[uq.Query.Key];
            }

            using (DisconnectedTools.SaveAndRestoreNextId(table))
            using (OperationLogic.AllowSave<UserChartDN>())
            {
                Administrator.SaveListDisableIdentity(list);
            }

            return list.Count;
        }
    }
}