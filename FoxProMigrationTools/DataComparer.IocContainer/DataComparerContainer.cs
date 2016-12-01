using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataComparer.Common;
using DataComparer.Common.Contracts;
using DataComparer.Common.Domain;
using DataComparer.Dal;
using StructureMap;

namespace DataComparer.IocContainer
{
    public static class DataComparerContainer
    {
        private static readonly Lazy<Container> ContainerBuilder = new Lazy<Container>(GetDefaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Instance
        {
            get { return ContainerBuilder.Value; }
        }

        private static Container GetDefaultContainer()
        {
            return new Container(c =>
            {
                c.For<IDataProvider>().Add<DbfDataProvider>().Named(Constants.Dbf);
                c.For<IDataProvider>().Add<SqlDataProvider>().Named(Constants.Sql);
                c.For<IXmlDataProvider>().Add<XmlDataProvider>();
                c.For<IDataTableComparer>().Add<DataTableComparer>();
            });
        }
    }
}
