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
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese nombre");
                return;
            }

            if (db.Cargos.Count() >= 100)
            {
                MessageBox.Show("No se pueden agregar más de 100 cargos");
                return;
            }

            try
            {
                Cargos car = new Cargos();

                car.Nombre = txtNombre.Text.Trim();

                db.Cargos.Add(car);
                db.SaveChanges();

                dgvCargo.DataSource = db.Cargos.ToList();

                MessageBox.Show("Cargo agregado correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
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

            var car = db.Cargos.Find(id);

            if (car == null)
            {
                MessageBox.Show("No existe");
                return;
            }

            car.Nombre = txtNombre.Text;

            db.SaveChanges();

            MessageBox.Show("Cargo actualizado correctamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

            MessageBox.Show("Cargo eliminado correctamente", "Eliminación", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dgvCargo.DataSource = db.Cargos.ToList();
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lista = db.Cargos.ToList();

                    using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        Document doc = new Document(PageSize.A4);
                        PdfWriter.GetInstance(doc, fs);

                        doc.Open();

                        PdfPTable tabla = new PdfPTable(2);

                        tabla.AddCell("ID");
                        tabla.AddCell("Nombre");

                        foreach (var c in lista)
                        {
                            tabla.AddCell(c.CargoID.ToString());
                            tabla.AddCell(c.Nombre);
                        }

                        doc.Add(tabla);
                        doc.Close();
                    }

                    MessageBox.Show("PDF de Cargos exportado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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

                        foreach (var c in db.Cargos.ToList())
                        {
                            sw.WriteLine(c.CargoID + ";" + c.Nombre);
                        }
                    }

                    MessageBox.Show("CSV de Cargos exportado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }
    }
}
