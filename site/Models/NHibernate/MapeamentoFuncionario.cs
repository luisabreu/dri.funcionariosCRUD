using FluentNHibernate.Mapping;

namespace site.Models.NHibernate {
    public class MapeamentoFuncionario : ClassMap<Funcionario> {
        public MapeamentoFuncionario() {
            Table("Funcionarios");
            Not.LazyLoad();
            Id(f => f.IdFuncionario, "Id")
               
                .GeneratedBy.Identity();
            Version(f => f.Versao);
            Map(f => f.Nome)
               
                .Not.Nullable();
            Map(f => f.Nif)
              
                .Not.Nullable();

            HasMany(f => f.Contactos)
               
                .AsBag()
                .Table("Contactos")
                .KeyColumn("IdFuncionario")
                .Component(c => {
                    c.Map(ct => ct.TipoContacto)
                        .CustomType<int>()
                        .CustomSqlType("integer");
                    c.Map(ct => ct.Valor);
                })
                .Cascade.All()
                .Not.LazyLoad();

            References(f => f.TipoFuncionario, "IdTipoFuncionario")
                .Cascade.None()
                .Not.LazyLoad();
        }
    }
}