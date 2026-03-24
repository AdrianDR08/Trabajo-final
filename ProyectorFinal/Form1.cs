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
    public partial class Form1 : Form
    {
        ProyectoFinalEntities2 db = new ProyectoFinalEntities2();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbDepartamento.DataSource = db.Departamentos.ToList();
            cmbDepartamento.DisplayMember = "Nombre";
            cmbDepartamento.ValueMember = "DepartamentoID";

            cmbCargo.DataSource = db.Cargos.ToList();
            cmbCargo.DisplayMember = "Nombre";
            cmbCargo.ValueMember = "CargoID";

            cmbEstado.Items.Add("Vigente");
            cmbEstado.Items.Add("No vigente");
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            dgvEmpleados.DataSource = db.Empleados.ToList();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int id;
            decimal salario;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            if (!decimal.TryParse(txtSalario.Text, out salario))
            {
                MessageBox.Show("Salario inválido");
                return;
            }

            if (txtNombre.Text == "")
            {
                MessageBox.Show("Ingrese nombre");
                return;
            }

            var existe = db.Empleados.Find(id);

            if (existe != null)
            {
                MessageBox.Show("ID ya existe");
                return;
            }

            Empleados emp = new Empleados();

            emp.ID = id;
            emp.Nombre = txtNombre.Text;
            emp.DepartamentoID = Convert.ToInt32(cmbDepartamento.SelectedValue);
            emp.CargoID = Convert.ToInt32(cmbCargo.SelectedValue);
            emp.FechaInicio = dtpFecha.Value;
            emp.Salario = salario;
            emp.Estado = cmbEstado.Text;

            db.Empleados.Add(emp);
            db.SaveChanges();

            dgvEmpleados.DataSource = db.Empleados.ToList();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int id;
            decimal salario;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            if (!decimal.TryParse(txtSalario.Text, out salario))
            {
                MessageBox.Show("Salario inválido");
                return;
            }

            var emp = db.Empleados.Find(id);

            if (emp == null)
            {
                MessageBox.Show("Empleado no existe");
                return;
            }

            emp.Nombre = txtNombre.Text;
            emp.DepartamentoID = Convert.ToInt32(cmbDepartamento.SelectedValue);
            emp.CargoID = Convert.ToInt32(cmbCargo.SelectedValue);
            emp.FechaInicio = dtpFecha.Value;
            emp.Salario = salario;
            emp.Estado = cmbEstado.Text;

            db.SaveChanges();

            dgvEmpleados.DataSource = db.Empleados.ToList();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(txtID.Text, out id))
            {
                MessageBox.Show("ID inválido");
                return;
            }

            var emp = db.Empleados.Find(id);

            if (emp == null)
            {
                MessageBox.Show("Empleado no existe");
                return;
            }

            db.Empleados.Remove(emp);
            db.SaveChanges();

            dgvEmpleados.DataSource = db.Empleados.ToList();
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvEmpleados.Rows[e.RowIndex];

                txtID.Text = fila.Cells["ID"].Value.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                cmbDepartamento.SelectedValue = fila.Cells["DepartamentoID"].Value;
                cmbCargo.SelectedValue = fila.Cells["CargoID"].Value;
                dtpFecha.Value = Convert.ToDateTime(fila.Cells["FechaInicio"].Value);
                txtSalario.Text = fila.Cells["Salario"].Value.ToString();
                cmbEstado.Text = fila.Cells["Estado"].Value.ToString();
            }
        }
    }

}

