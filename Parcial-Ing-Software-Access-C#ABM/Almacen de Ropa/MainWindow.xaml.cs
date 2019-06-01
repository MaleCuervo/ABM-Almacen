using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;

namespace AlmacenDR
{
    /// <summary>

    
    public partial class MainWindow : Window
    {
        OleDbConnection con;
        DataTable dt;

        public MainWindow()
        {
            InitializeComponent();

            //Conexion a la base de datos
            con = new OleDbConnection();
            con.ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\AlmacenDB.mdb";
            CargaDatos();
        }

        //Se carga la informacion de la DB en la Data Grid
        private void CargaDatos()
        {
            OleDbCommand cmd = new OleDbCommand();
            //Verifica si existen datos para mostrar
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            cmd.CommandText = "select * from tbRopa";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            VistaDatos.ItemsSource = dt.AsDataView();

            if (dt.Rows.Count > 0)
            {
                labelsindatos.Visibility = System.Windows.Visibility.Hidden; //Si existen datos en la DB el label no se muestra
                VistaDatos.Visibility = System.Windows.Visibility.Visible; //Se hace visible la Data Grid
            }
            else
            {
                labelsindatos.Visibility = System.Windows.Visibility.Visible; //De no haber datos cargados en la DB, aparece el label con el mensaje "No se encontraron Registros
                VistaDatos.Visibility = System.Windows.Visibility.Hidden;//Se oculta la Data Grid
            }

        }

        //Agregar Registros a la Data grid / Base de Datos
        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            OleDbCommand cmd = new OleDbCommand();
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            //el evento click verifica la conexion a la base de datos, de no estar conectada, ejecuta con.Open();

            if (txtCodigo.Text != "") //Verifica que se introduzca algun valor en la textbox codigo
            {
                if (txtCodigo.IsEnabled == true)//Verifica que esté habilitado escribir en el campo codigo
                {
                    if (ComboRopa.Text != "Tipo Prenda") //verifica que se seleccione algun tipo de prenda
                    {
                        cmd.CommandText = "insert into tbRopa (Codigo,Nombre,Prenda,Cantidad,Precio) Values(" + txtCodigo.Text + ",'" + txtNombre.Text + "','" + ComboRopa.Text + "'," + txtCantidad.Text + ",'" + txtPrecio.Text + "')";
                        cmd.ExecuteNonQuery();
                        CargaDatos();
                        MessageBox.Show("Nueva Compra agregada");
                        ClearAll();

                    }
                    else
                    {
                        MessageBox.Show("Por favor seleccione el tipo de prenda");
                    }
                    //else para seleccion de prenda
                }
                else
                {
                    cmd.CommandText = "update tbRopa set Nombre='" + txtNombre.Text + "',Prenda='" + ComboRopa.Text + "',Cantidad=" + txtCantidad.Text + ",Precio='" + txtPrecio.Text + "' where Codigo =" + txtCodigo.Text;
                    cmd.ExecuteNonQuery();
                    CargaDatos();
                    MessageBox.Show("Se han guardado las modificaciones");
                    ClearAll();
                    //else para EDITAR: si codigo está en "false" significa que se esta editando un registro 
                }
            }
            else
            {
                MessageBox.Show("Por favor Ingrese un Código."); //else para codigo vacio
            }
        }

        //Limpiar todas las Casillas:  Cancelar Edicion - Nueva Compra
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            ComboRopa.SelectedIndex = 0;
            txtCantidad.Text = "";
            txtPrecio.Text = "";
            btnNuevo.Content = "Nuevo";
            txtCodigo.IsEnabled = true;
        }

        //Editar Registros
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (VistaDatos.SelectedItems.Count > 0)
                //If que verifica si está seleccionado un registro
            {
                DataRowView row = (DataRowView)VistaDatos.SelectedItems[0];
                txtCodigo.Text = row["Codigo"].ToString();
                txtNombre.Text = row["Nombre"].ToString();
                ComboRopa.Text = row["Prenda"].ToString();
                txtCantidad.Text = row["Cantidad"].ToString();
                txtPrecio.Text = row["Precio"].ToString();
                txtCodigo.IsEnabled = false;
                btnNuevo.Content = "Guardar";
            }
            else
            {
                MessageBox.Show("Seleccione un Cliente de la lista");
                //else para seleccion de registro
            }
        }

        //Borrar registros de la grilla
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (VistaDatos.SelectedItems.Count > 0)
                //verifica que se seleccione algun elemento
            {
                DataRowView row = (DataRowView)VistaDatos.SelectedItems[0];

                OleDbCommand cmd = new OleDbCommand();
                if (con.State != ConnectionState.Open)
                    con.Open();
                cmd.Connection = con;
                cmd.CommandText = "delete from tbRopa where Codigo=" + row["Codigo"].ToString();
                cmd.ExecuteNonQuery();
                CargaDatos();
                MessageBox.Show("Se ha eliminado la compra");
                ClearAll();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
            //else para seleccionar registro
        }

        //Salida
        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ComboRopa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}