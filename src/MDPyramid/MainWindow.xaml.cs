using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MDPyramid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            edInput.Text = Test<PrimitiveMethod>.QuestionStr;
        }

        private bool initialRefresh = true;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (initialRefresh)
            {
                initialRefresh = false;
                RunPrimitive();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                edSum.Clear();
                edPath.Clear();
                if (String.IsNullOrEmpty(edInput.Text)) return;
                var m = new Method3();
                int[] res = m.Run(edInput.Text);
                edSum.Text = res.Sum().ToString();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void RunPrimitive()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                edSum.Clear();
                edPath.Clear();
                if (String.IsNullOrEmpty(edInput.Text)) return;
                var m = new PrimitiveMethod();
                int[] res = m.Run(edInput.Text);
                edSum.Text = res.Sum().ToString();
                edPath.Text = res.ToStringAggregate();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RunPrimitive();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var sw = Stopwatch.StartNew();
        //    Test<Method2>.All(false);
        //    MessageBox.Show(sw.Elapsed.ToString());
        //    sw = Stopwatch.StartNew();
        //    Test<PrimitiveMethod>.All(false);
        //    MessageBox.Show(sw.Elapsed.ToString());
        //    sw = Stopwatch.StartNew();
        //    Test<Method3>.All(false);
        //    MessageBox.Show(sw.Elapsed.ToString());
        //}
    }
}
