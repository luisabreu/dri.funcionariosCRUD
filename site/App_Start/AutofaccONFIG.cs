using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using NHibernate;
using site.Models;
using site.Models.NHibernate;

namespace site {
    public class AutofacConfig {
        public static void RegisterForMvc() {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(typeof (Funcionario).Assembly, typeof (TipoContacto).Assembly)
                .AsImplementedInterfaces()
                .AsSelf();

            OverrideDependencyRegistration(builder);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }

        private static ISessionFactory _fabrica = new GestorTransacoes().ObtemFabricaSessoes();
        private static void OverrideDependencyRegistration(ContainerBuilder builder) {
            builder.Register(c => _fabrica.OpenSession())
                .As<ISession>()
                .InstancePerRequest();
        }
    }
}