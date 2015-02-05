using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Criterion;
using site.Models;
using site.Models.ViewModel;

namespace site.Controllers {
    public class HomeController : Controller {
        private static Regex _nifRegex = new Regex(@"^\d{9}$");
        private readonly ISession _session;

        public HomeController(ISession session) {
            Contract.Requires(session != null);
            Contract.Ensures(_session != null);
            _session = session;
        }

        public ActionResult Index(string nifOuNome) {
            if (string.IsNullOrEmpty(nifOuNome)) {
                return View(new DadosPesquisa {NifOuNome = ""});
            }
            using (var tran = _session.BeginTransaction()) {
                var query = _session.QueryOver<Funcionario>();
                query = ENif(nifOuNome)
                    ? query.Where(f => f.Nif == nifOuNome)
                    : query.Where(f => f.Nome.IsLike("%" + nifOuNome.Replace(" ", "%") + "%"));
                return View(new DadosPesquisa {
                    Funcionarios = query.List<Funcionario>(),
                    NifOuNome = nifOuNome,
                    PesquisaEfetuada = true
                });
            }
        }

        public ActionResult NovoFuncionario() {
            IEnumerable<TipoFuncionario> tipos = null;
            using (var tran = _session.BeginTransaction()) {
                tipos = _session.QueryOver<TipoFuncionario>()
                    .List<TipoFuncionario>();
            }
            var funcionario = new Funcionario {
                Contactos = new List<Contacto>(),
                IdFuncionario = 0,
                Nif = "",
                Nome = "",
                TipoFuncionario = tipos.FirstOrDefault(),
                Versao = 0
            };
            return View("fichafuncionario", new DadosFichaFormulario {
                ENovo = true,
                Funcionario = funcionario,
                TiposFuncionario = tipos
            });
        }

        public ActionResult FichaFuncionario(int id) {
            using (var tran = _session.BeginTransaction()) {
                var funcionario = _session.Load<Funcionario>(id);
                var tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        public ActionResult DadosGerais(int id, int versao, string nome, string nif, int tipoFuncionario) {
            return id == 0 && versao == 0
                ? AdicionaNovoFuncionario(nome, nif, tipoFuncionario)
                : ModificaDadosGerais(id, versao, nome, nif, tipoFuncionario);
        }

        private ActionResult ModificaDadosGerais(int id, int versao, string nome, string nif, int tipoFuncionario) {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _session.QueryOver<TipoFuncionario>()
                    .List<TipoFuncionario>();
                var tipo = tipos.First(tf => tf.Id == tipoFuncionario);
                var funcionario = _session.Load<Funcionario>(id);
                Contract.Assume(funcionario != null);

                funcionario.Nif = nif;
                funcionario.Nome = nome;
                funcionario.TipoFuncionario = tipo;

                if (NifDuplicado(nif, 0)) {
                    ModelState.AddModelError("nif", "NIF já foi associado a outro contribuinte");
                }
                else {
                    _session.SaveOrUpdate(funcionario);
                    tran.Commit();
                }
                
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private ActionResult AdicionaNovoFuncionario(string nome, string nif, int tipoFuncionario) {
            using (var tran = _session.BeginTransaction()) {

                var ocorreuErro = false;
                var tipos = _session.QueryOver<TipoFuncionario>()
                    .List<TipoFuncionario>();
                var tipo = tipos.First(tf => tf.Id == tipoFuncionario);
                var funcionario = new Funcionario {
                    Nome = nome,
                    Nif = nif,
                    TipoFuncionario = tipo,
                    Contactos = new List<Contacto>()
                };
                if (ocorreuErro = NifDuplicado(nif, 0)){
                    ModelState.AddModelError("nif", "NIF já foi associado a outro contribuinte");
                }
                else {
                    _session.SaveOrUpdate(funcionario);
                    tran.Commit();
                }
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = ocorreuErro,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private bool NifDuplicado(string nif, int id) {
            var total = _session.QueryOver<Funcionario>()
                .Where(f => f.Nif == nif && f.IdFuncionario != id)
                .RowCount();
            return total > 0;
        }

        public ActionResult AdicionaContacto(int id, int versao, string contacto) {
            using (var tran = _session.BeginTransaction()) {
                var funcionario = _session.Load<Funcionario>(id);
                Contract.Assume(funcionario.Versao == versao);
                var ct = ObtemContacto(contacto);
                Contract.Assume(ct != null);
                funcionario.Contactos.Add(ct);
                var tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                tran.Commit();
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private Contacto ObtemContacto(string contacto) {
            if (new Regex(@"^\d{9}$").IsMatch(contacto)) {
                return new Contacto {
                    TipoContacto = TipoContacto.Telefone,
                    Valor = contacto
                };
            }
            if (new Regex(@"^\d{4}$").IsMatch(contacto)) {
                return new Contacto {
                    TipoContacto = TipoContacto.Extensao,
                    Valor = contacto
                };
            }
            try {
                new MailAddress(contacto);
                return new Contacto {
                    TipoContacto = TipoContacto.Email,
                    Valor = contacto
                };
            }
            catch (Exception) {
                return null;
            }
        }

        public ActionResult EliminaContacto(int id, int versao, string contacto) {
            using (var tran = _session.BeginTransaction()) {
                var funcionario = _session.Load<Funcionario>(id);
                Contract.Assume( funcionario != null && funcionario.Versao == versao);

                for (var i = 0; i < funcionario.Contactos.Count; i++) {
                    var ct = funcionario.Contactos[i];
                    if (ct.Valor == contacto) {
                        funcionario.Contactos.RemoveAt(i);
                        break;
                    } 
                }
                _session.SaveOrUpdate(funcionario);
                var tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                tran.Commit();
                return View("fichafuncionario", new DadosFichaFormulario {
                    ENovo = false,
                    Funcionario = funcionario,
                    TiposFuncionario = tipos
                });
            }
        }

        private bool ENif(string nomeOuNif) {
            return _nifRegex.IsMatch(nomeOuNif);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
        }
    }
}