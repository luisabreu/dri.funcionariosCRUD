using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NHibernate;

namespace site.Models.ViewModel {
    public class RepositorioTiposFuncionario {
        private readonly ISession _session;

        public RepositorioTiposFuncionario(ISession session) {
            Contract.Requires(session != null);
            Contract.Ensures(_session != null);
            _session = session;
        }

        public IEnumerable<TipoFuncionario> ObtemTodosTipos() {
            return _session.QueryOver<TipoFuncionario>()
                    .List<TipoFuncionario>();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
        }
    }
}