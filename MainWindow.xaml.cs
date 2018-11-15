using GestionEmpleados.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Linq;
using System;

namespace GestionEmpleados
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmpresaXYZEntities dBContext = null;
        private Branch sucursal = null;
        //private Employee employee = null;
        private IList<Employee> listaEmpleados = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            dBContext = new EmpresaXYZEntities();
            IList<Branch> branch = (from a in dBContext.Branches
                                    orderby a.BranchName 
                                    select a).ToList();
            CbSucursales.ItemsSource = branch;
            CbSucursales.DisplayMemberPath = "BranchName";
            CbSucursales.SelectedValuePath = "BranchID";
        }

        private void CbSucursales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sucursal = CbSucursales.SelectedItem as Branch;
            //this.dBContext.Entry<>

            listaEmpleados = (from a in dBContext.Employees
                                    where a.Branch == sucursal.BranchID
                                    select a
                                    ).ToList();

            if (listaEmpleados.Count == 0)
                MessageBox.Show($"La sucursal:{sucursal.BranchName} no tiene empleados", "Alerta", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            AtualizarListaEmpleados();
            //LvEmpleados.



        }

        private void AtualizarListaEmpleados()
        {
            LvEmpleados.ItemsSource = null;
            LvEmpleados.ItemsSource = listaEmpleados;
            LvEmpleados.DisplayMemberPath = "FirstName";
           
        }

        private void LvEmpleados_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    Employee empleado = LvEmpleados.SelectedItem as Employee;
                    EditarEmpleado(empleado);
                    break;
                case System.Windows.Input.Key.Insert:
                    InsertNuevoEmpleado();
                    break;
                case System.Windows.Input.Key.Delete:
                    empleado = LvEmpleados.SelectedItem as Employee;
                    BorreEmpleado(empleado);
                    break;
                default:
                    break;
            }
        }

        private void BorreEmpleado(Employee empleado)
        {
            MessageBoxResult response = MessageBox.Show(
               String.Format("Remove {0} {1}", empleado.FirstName , empleado.LastName),
               "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question,
               MessageBoxResult.No);
            // If the user clicked Yes, remove the empleado from the database
            if (response == MessageBoxResult.Yes)
            {
                listaEmpleados.Remove(empleado);
                dBContext.Employees.Remove(empleado);
                SavalBBDD.IsEnabled = true;
                AtualizarListaEmpleados();

            }
        }

        private void InsertNuevoEmpleado()
        {
            // Use the StudentsForm to get the details of the student from the user
            Empleados sf = new Empleados();

            // Set the title of the form to indicate which class the student will be added to (the class for the currently selected teacher)
            sf.Title = $"Nuevo empleado para la sucursal:{sucursal.BranchName}";

            // Display the form and get the details of the new student
            if (sf.ShowDialog().Value)
            {
                // When the user closes the form, retrieve the details of the student from the form
                // and use them to create a new Student object
                Employee newEmpleado = new Employee();
                newEmpleado.FirstName = sf.Nombre_Empleado.Text;
                newEmpleado.LastName = String.Empty;
                newEmpleado.Branch = Int32.Parse(sf.ID_Sucursal.Text);
                DateTime vente = DateTime.Parse("1/1/1998");
                newEmpleado.DateOfBirth = vente;
                //newEmpleado.DateOfBirth = DateTime.Parse(sf.dateOfBirth.Text);

                // Assign the new student to the current teacher
                sucursal.Employees.Add(newEmpleado);

                // Add the student to the list displayed on the form
                listaEmpleados.Add(newEmpleado);
                AtualizarListaEmpleados();


                // Enable saving (changes are not made permanent until they are written back to the database)
                SavalBBDD.IsEnabled = true;
            }
        }

        private void EditarEmpleado(Employee empleado)
        {
            // Use the empleadosForm to display and edit the details of the empleado
            Empleados em = new Empleados();

            // Set the title of the form and populate the fields on the form with the details of the empleado           
            em.Title = "Edit empleado Details";
            em.Nombre_Empleado.Text = empleado.FirstName ;
            em.IDEmpleado.Text =  empleado.EmployeeID.ToString();
            em.ID_Sucursal.Text = empleado.Branch.ToString();
            // Display the form
            if (em.ShowDialog().Value)
            {
                // When the user closes the form, copy the details back to the empleado
                empleado.FirstName = em.Nombre_Empleado.Text;
                empleado.LastName = String.Empty;
                empleado.Branch = Int32.Parse(em.ID_Sucursal.Text);
                DateTime vente = DateTime.Parse("1/1/1998") ;
                empleado.DateOfBirth = vente;
                AtualizarListaEmpleados();
                // Enable saving (changes are not made permanent until they are written back to the database)
                SavalBBDD.IsEnabled = true;
            }
        }

        private void NewEmployXML_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
