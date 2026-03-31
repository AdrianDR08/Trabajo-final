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
    public partial class Cargo : Form
    {
        ProyectoFinalEntities2 db = new ProyectoFinalEntities2();
        public Cargo()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow fila = dgvCargo.Rows[e.RowIndex];

            txtID.Text = fila.Cells["CargoID"].Value.ToString();
            txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
        }

        private void Cargo_Load(object sender, EventArgs e)
        {

        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            dgvCargo.DataSource = db.Cargos.ToList();
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

            var existe = db.Cargos.Find(id);

            if (existe != null)
            {
                MessageBox.Show("ID ya existe");
                return;
            }

            Cargos car = new Cargos();

            car.CargoID = id;
            car.Nombre = txtNombre.Text;

            db.Cargos.Add(car);
            db.SaveChanges();

            dgvCargo.DataSource = db.Cargos.ToList();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            var car = db.Cargos.Find(id);

            if (car == null)
            {
                MessageBox.Show("No existe");
                return;
            }

            car.Nombre = txtNombre.Text;

            db.SaveChanges();

            dgvCargo.DataSource = db.Cargos.ToList();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCargo.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro");
                return;
            }

            int id = Convert.ToInt32(dgvCargo.CurrentRow.Cells["CargoID"].Value);

            var car = db.Cargos.Find(id);

            if (car == null)
            {
                MessageBox.Show("No existe");
                return;
            }

            db.Cargos.Remove(car);
            db.SaveChanges();

            dgvCargo.DataSource = db.Cargos.ToList();
        }
    }
}
