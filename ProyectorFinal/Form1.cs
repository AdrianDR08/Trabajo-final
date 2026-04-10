using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;
using System.Linq;
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
        

        private void CargarEmpleados()
        {
            var lista = db.Empleados.ToList()
            .Select(emp => new
            {
                emp.ID,
                emp.Nombre,
                Departamento = db.Departamentos
                    .Where(d => d.DepartamentoID == emp.DepartamentoID)
                    .Select(d => d.Nombre)
                    .FirstOrDefault(),

                Cargo = db.Cargos
                    .Where(c => c.CargoID == emp.CargoID)
                    .Select(c => c.Nombre)
                    .FirstOrDefault(),

                emp.FechaInicio,
                emp.Salario,
                emp.Estado
            })
            .ToList();

            dgvEmpleados.DataSource = lista;
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

            CargarEmpleados();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarEmpleados();
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

            CargarEmpleados();

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

            MessageBox.Show("Empleado actualizado correctamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CargarEmpleados();
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

            MessageBox.Show("Empleado eliminado correctamente", "Eliminación", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CargarEmpleados();
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvEmpleados.Rows[e.RowIndex];

            txtID.Text = fila.Cells["ID"].Value.ToString();
            txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
            txtSalario.Text = fila.Cells["Salario"].Value.ToString();
            cmbEstado.Text = fila.Cells["Estado"].Value.ToString();
            cmbDepartamento.Text = fila.Cells["Departamento"].Value.ToString();
            cmbCargo.Text = fila.Cells["Cargo"].Value.ToString();
            dtpFecha.Value = Convert.ToDateTime(fila.Cells["FechaInicio"].Value);
        }
        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {

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
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    for (int i = 0; i < dgvEmpleados.Columns.Count; i++)
                    {
                        sw.Write(dgvEmpleados.Columns[i].HeaderText);

                        if (i < dgvEmpleados.Columns.Count - 1)
                            sw.Write(",");
                    }

                    sw.WriteLine();

                    foreach (DataGridViewRow fila in dgvEmpleados.Rows)
                    {
                        if (!fila.IsNewRow)
                        {
                            for (int i = 0; i < dgvEmpleados.Columns.Count; i++)
                            {
                                var valor = fila.Cells[i].Value;

                                if (valor != null)
                                {
                                    if (valor is DateTime)
                                        sw.Write(((DateTime)valor).ToString("dd/MM/yyyy"));
                                    else
                                        sw.Write(valor.ToString());
                                }

                                if (i < dgvEmpleados.Columns.Count - 1)
                                    sw.Write(",");
                            }

                            sw.WriteLine();
                        }
                    }
                }

                MessageBox.Show("Archivo CSV exportado");
            }
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, fs);

                        doc.Open();

                        PdfPTable tabla = new PdfPTable(dgvEmpleados.Columns.Count);

                        foreach (DataGridViewColumn col in dgvEmpleados.Columns)
                        {
                            tabla.AddCell(col.HeaderText);
                        }

                        foreach (DataGridViewRow fila in dgvEmpleados.Rows)
                        {
                            if (!fila.IsNewRow)
                            {
                                foreach (DataGridViewCell celda in fila.Cells)
                                {
                                    if (celda.Value != null)
                                    {
                                        if (celda.Value is DateTime)
                                            tabla.AddCell(((DateTime)celda.Value).ToString("dd/MM/yyyy"));
                                        else
                                            tabla.AddCell(celda.Value.ToString());
                                    }
                                    else
                                    {
                                        tabla.AddCell("");
                                    }
                                }
                            }
                        }

                        doc.Add(tabla);
                        doc.Close();
                    }

                    MessageBox.Show("PDF exportado correctamente");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear PDF: " + ex.Message);
                }
            }
        }

}
}

