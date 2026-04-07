using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private void btnMostrar_Click(object sender, EventArgs a)
        {
            var lista = db.Empleados.ToList()
            .Select(e => new
            {
                e.ID,
                e.Nombre,
                Departamento = e.DepartamentoID,
                Cargo = e.CargoID,
                e.FechaInicio,
                e.Estado,
                e.Salario
            })
            .ToList();

            dgvEmpleados.DataSource = lista;
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

            decimal afp = salario * 0.0287m;
            decimal ars = salario * 0.0304m;
            decimal isr = salario * 0.10m;

            DateTime inicio = dtpFecha.Value;
            DateTime hoy = DateTime.Now;

            int años = hoy.Year - inicio.Year;
            int meses = hoy.Month - inicio.Month;

            if (meses < 0)
            {
                años--;
                meses += 12;
            }

            string tiempo = años + " años y " + meses + " meses";

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

            MessageBox.Show("AFP: " + afp + " | ARS: " + ars + " | ISR: " + isr + " | Tiempo: " + tiempo);
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
                cmbDepartamento.SelectedValue = fila.Cells["Departamento"].Value;
                cmbCargo.SelectedValue = fila.Cells["Cargo"].Value;
                dtpFecha.Value = Convert.ToDateTime(fila.Cells["FechaInicio"].Value);
                txtSalario.Text = fila.Cells["Salario"].Value.ToString();
                cmbEstado.Text = fila.Cells["Estado"].Value.ToString();
            }
        }

        private void txtSalario_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd.FileName);

                for (int i = 0; i < dgvEmpleados.Columns.Count; i++)
                {
                    sw.Write(dgvEmpleados.Columns[i].HeaderText + ",");
                }

                sw.WriteLine();

                foreach (DataGridViewRow fila in dgvEmpleados.Rows)
                {
                    if (!fila.IsNewRow)
                    {
                        for (int i = 0; i < dgvEmpleados.Columns.Count; i++)
                        {
                            sw.Write(fila.Cells[i].Value + ",");
                        }

                        sw.WriteLine();
                    }
                }

                sw.Close();

                MessageBox.Show("Archivo CSV exportado");
            }
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {

        }
    }

}

