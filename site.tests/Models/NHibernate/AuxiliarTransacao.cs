using System;
using NHibernate;
using site.Models.NHibernate;

namespace site.tests.Models.NHibernate {
    public class AuxiliarTransacao : GestorTransacoes {
        protected override string ObtemCnnString() {
            return @"Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|bd.mdf;Integrated Security=true";
        }

        public void ExecutaTransacao(Action<ISession, ITransaction> acao) {
            using (var session = ObtemFabricaSessoes().OpenSession()) {
                using (var tran = session.BeginTransaction()) {
                    acao(session, tran);
                }
            }
        }
    }
}