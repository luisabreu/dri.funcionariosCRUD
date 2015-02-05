using System.Diagnostics.Contracts;
using NHibernate;

namespace site.Models.ViewModel {
    [ContractClass(typeof (ContratoVerificadorNif))]
    public interface IVerificadorNif {
        bool NifDuplicado(string nif, int id);
    }

    [ContractClassFor(typeof (IVerificadorNif))]
    internal abstract class ContratoVerificadorNif : IVerificadorNif {
        public bool NifDuplicado(string nif, int id) {
            return default(bool);
        }
    }

    public class VerificadorNif : IVerificadorNif {
        private readonly ISession _session;

        public VerificadorNif(ISession session) {
            Contract.Requires(session != null);
            Contract.Ensures(_session != null);
            _session = session;
        }

        public bool NifDuplicado(string nif, int id) {
            var total = _session.QueryOver<Funcionario>()
                .Where(f => f.Nif == nif && f.IdFuncionario != id)
                .RowCount();
            return total > 0;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
        }
    }
}