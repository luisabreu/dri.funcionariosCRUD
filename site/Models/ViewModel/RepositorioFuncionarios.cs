using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using NHibernate;
using NHibernate.Criterion;

namespace site.Models.ViewModel {
    public class RepositorioFuncionarios {
        private static Regex _nifRegex = new Regex(@"^\d{9}$");
        private readonly ISession _session;
        private readonly IVerificadorNif _verificador;

        public RepositorioFuncionarios(ISession session, IVerificadorNif verificador) {
            Contract.Requires(session != null);
            Contract.Requires(verificador != null);
            Contract.Ensures(_session != null);
            _session = session;
            _verificador = verificador;
        }

        public IEnumerable<Funcionario> Pesquisa(string nifOuNome) {
            var query = _session.QueryOver<Funcionario>();
            query = ENif(nifOuNome)
                ? query.Where(f => f.Nif == nifOuNome)
                : query.Where(f => f.Nome.IsLike("%" + nifOuNome.Replace(" ", "%") + "%"));
            return query.List<Funcionario>();
        }

        private bool ENif(string nomeOuNif) {
            return _nifRegex.IsMatch(nomeOuNif);
        }

        public Funcionario ObtemFuncionario(int id) {
            var funcionario = _session.Load<Funcionario>(id);
            return funcionario;
        }

        public void Grava(Funcionario funcionario) {
            Contract.Requires(funcionario != null);
            if (_verificador.NifDuplicado(funcionario.Nif, funcionario.IdFuncionario)) {
                throw new InvalidOperationException();
            }
            _session.SaveOrUpdate(funcionario);
        }

        

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
            Contract.Invariant(_verificador != null);
        }
    }
}