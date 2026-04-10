using iTextSharp.text;
using iTextSharp.text.pdf;
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
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Ingrese nombre");
                    return;
                }

                try
                {
                    Departamentos depart = new Departamentos();

                    dep.Nombre = txtNombre.Text.Trim();

                    db.Departamentos.Add(dep);
                    db.SaveChanges();

                    dgvDepartamentos.DataSource = db.Departamentos.ToList();

                    MessageBox.Show("Departamento agregado correctamente");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
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

            MessageBox.Show("Departamento actualizado correctamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dgvDepartamentos.DataSource = db.Departamentos.ToList();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvDepartamentos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro");
                return;
            }

            int id = Convert.ToInt32(dgvDepartamentos.CurrentRow.Cells[0].Value);

            var dep = db.Departamentos.Find(id);

            if (dep == null)
            {
                MessageBox.Show("El departamento no existe");
                return;
            }

            var tieneEmpleados = db.Empleados.Any(x => x.DepartamentoID == id);

            if (tieneEmpleados)
            {
                MessageBox.Show("No se puede eliminar este departamento porque tiene empleados asociados");
                return;
            }

            try
            {
                db.Departamentos.Remove(dep);
                db.SaveChanges();

                dgvDepartamentos.DataSource = db.Departamentos.ToList();

                MessageBox.Show("Departamento eliminado correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
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

        private void btnCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine("ID;Nombre");

                        foreach (var d in db.Departamentos.ToList())
                        {
                            sw.WriteLine(d.DepartamentoID + ";" + d.Nombre);
                        }
                    }

                    MessageBox.Show("CSV de Departamentos exportado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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
                    var lista = db.Departamentos.ToList();

                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, fs);

                        doc.Open();

                        PdfPTable tabla = new PdfPTable(2);

                        tabla.AddCell("ID");
                        tabla.AddCell("Nombre");

                        foreach (var d in lista)
                        {
                            tabla.AddCell(d.DepartamentoID.ToString());
                            tabla.AddCell(d.Nombre);
                        }

                        doc.Add(tabla);
                        doc.Close();
                    }

                    MessageBox.Show("PDF de Departamentos exportado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
}
    }
}

