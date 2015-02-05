using System;
using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;

namespace site.Models.NHibernate {
    public class GestorTransacoes  {
        public ISessionFactory ObtemFabricaSessoes() {
            var configuration = Fluently.Configure()
                .Database(ConfiguraDb());
            MapeiaTiposDeAssembly(configuration);
            return configuration.BuildConfiguration().BuildSessionFactory();
        }

        protected virtual string ObtemCnnString() {
            return ConfigurationManager.ConnectionStrings["bd"].ConnectionString;
        }

        public virtual IPersistenceConfigurer ConfiguraDb() {
            return MsSqlConfiguration.MsSql2008
                .ConnectionString(ObtemCnnString())
                .ShowSql();
        }

        protected virtual void MapeiaTiposDeAssembly(FluentConfiguration configuration) {
            //Autoimport set to false because there are 2 mappings
            configuration.Mappings(m => m.FluentMappings
                .Conventions.Setup(x => x.Add(AutoImport.Never()))
                .AddFromAssemblyOf<Funcionario>());
        }

        
    }
}