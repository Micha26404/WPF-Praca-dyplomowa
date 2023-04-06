using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity.Core.Objects;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.database;

namespace WPF
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Movies_Click(sender,e);
			Clients_Click(sender, e);
			Orders_Click(sender, e);
		}

		private void Movies_Click(object sender, RoutedEventArgs e)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = "Select * from movies";
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			MoviesCatalog.ItemsSource = dtRecord.DefaultView;
		}

		private void Orders_Click(object sender, RoutedEventArgs e)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = "Select * from orders";
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			OrdersCatalog.ItemsSource = dtRecord.DefaultView;
		}

		private void Clients_Click(object sender, RoutedEventArgs e)
		{
			string strConnection = Properties.Settings.Default.WPF_DBConnectionString;
			SqlConnection con = new SqlConnection(strConnection);

			SqlCommand sqlCmd = new SqlCommand();
			sqlCmd.Connection = con;
			sqlCmd.CommandType = CommandType.Text;
			sqlCmd.CommandText = "Select * from clients";
			SqlDataAdapter sqlDataAdap = new SqlDataAdapter(sqlCmd);

			DataTable dtRecord = new DataTable();
			sqlDataAdap.Fill(dtRecord);
			ClientsCatalog.ItemsSource = dtRecord.DefaultView;
		}
		private void Poster_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Poster.Visibility = Visibility.Hidden;
        }
		private void FilterTitle_MouseDoubleClick(object sender,MouseButtonEventArgs e)
		{
			FilterTitle.Text = "filter";
		}
		private void FilterYear_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterYear.Text = "filter";
		}
		private void FilterAge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterAge.Text = "filter";
		}
		private void FilterDuration_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterDuration.Text = "filter";
		}
		private void FilterLanguages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterLanguages.Text = "filter";
		}
		private void FilterDirectors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterDirectors.Text = "filter";
		}
		private void FilterFormats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterFormats.Text = "filter";
		}
		private void FilterPrice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterPrice.Text = "filter";
		}
		private void FilterCountries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCountries.Text = "filter";
		}
		private void FilterLeadActors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterLeadActors.Text = "filter";
		}
		private void FilterCopiesLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCopiesLeft.Text = "filter";
		}
		private void FilterCopiesTotal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			FilterCopiesTotal.Text = "filter";
		}

		private void Element_MediaEnded(object sender, RoutedEventArgs e)
		{

        }

		private void Element_MediaOpened(object sender, RoutedEventArgs e)
		{

		}

		private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void OnMouseDownPlayMedia(object sender, RoutedEventArgs e)
		{

		}

		private void OnMouseDownPauseMedia(object sender, RoutedEventArgs e)
		{

		}

		private void OnMouseDownStopMedia(object sender, RoutedEventArgs e)
		{

		}

		private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void Duration_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPremise_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLastName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFirstName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterEmail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterPhone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterLName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterFName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterDueDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterReturnDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		private void FilterRentDate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}
	}
}