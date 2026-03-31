using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectorFinal
{
    public partial class Departamento : Form
    {
        ProyectoFinalEntities2 db = new ProyectoFinalEntities2();
        public Departamento()
        {
            InitializeComponent();
        }

        private void Departamento_Load(object sender, EventArgs e)
        {

        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            dgvDepartamentos.DataSource = db.Departamentos.ToList();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            if (txtNombre.Text == "")
            {
                MessageBox.Show("Ingrese nombre");
                return;
            }

            var existe = db.Departamentos.Find(id);

            if (existe != null)
            {
                MessageBox.Show("ID ya existe");
                return;
            }

            Departamentos dep = new Departamentos();

            dep.DepartamentoID = id;
            dep.Nombre = txtNombre.Text;

            db.Departamentos.Add(dep);
            db.SaveChanges();

            dgvDepartamentos.DataSource = db.Departamentos.ToList();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            var dep = db.Departamentos.Find(id);

            if (dep == null)
            {
                MessageBox.Show("No existe");
                return;
            }

            dep.Nombre = txtNombre.Text;

            db.SaveChanges();

            dgvDepartamentos.DataSource = db.Departamentos.ToList();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            var dep = db.Departamentos.Find(id);

            if (dep == null)
            {
                MessageBox.Show("No existe");
                return;
            }

            db.Departamentos.Remove(dep);
            db.SaveChanges();

            dgvDepartamentos.DataSource = db.Departamentos.ToList();
        }

        private void dgvDepartamentos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvDepartamentos.Rows[e.RowIndex];

                txtID.Text = fila.Cells["DepartamentoID"].Value.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
            }
        }
    }
}

