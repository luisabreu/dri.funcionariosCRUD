using FluentNHibernate.Mapping;

namespace site.Models.NHibernate {
    public class MapeamentoTipoFuncionario : ClassMap<TipoFuncionario> {
        public MapeamentoTipoFuncionario() {
            Table("TipoFuncionario");
            Not.LazyLoad();
            ReadOnly();
            Id(funcionario => funcionario.Id)
                
                .Default(0)
                .GeneratedBy.Assigned();
            Map(tf => tf.Descricao)
                .Not.Nullable();
        }
    }
}