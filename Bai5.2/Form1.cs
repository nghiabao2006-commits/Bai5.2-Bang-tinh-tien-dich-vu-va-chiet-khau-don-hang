using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai5._2
{
    public partial class Form1 : Form
    {
        private BindingSource availableSource = new BindingSource();
        private BindingSource selectedSource = new BindingSource();

        private class ServiceItem
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public override string ToString()
            {
                return $"{Name} - {Price:C}";
            }
        }

        private Dictionary<string, List<ServiceItem>> categories = new Dictionary<string, List<ServiceItem>>();

        public Form1()
        {
            InitializeComponent();
            // Prepare data bindings
            lstAvailableServices.DataSource = availableSource;
            lstSelectedServices.DataSource = selectedSource;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            cboCategory.Items.AddRange(categories.Keys.ToArray());
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
        }

        private void LoadData()
        {
            // Define services for each category
            categories.Clear();
            categories["Khám bệnh"] = new List<ServiceItem>
            {
                new ServiceItem { Name = "Khám tổng quát", Price = 50m },
                new ServiceItem { Name = "Khám chuyên khoa", Price = 80m },
            };
            categories["Xét nghiệm"] = new List<ServiceItem>
            {
                new ServiceItem { Name = "Xét nghiệm máu", Price = 30m },
                new ServiceItem { Name = "Xét nghiệm nước tiểu", Price = 20m },
            };
            categories["Chụp X-Quang"] = new List<ServiceItem>
            {
                new ServiceItem { Name = "X-Quang ngực", Price = 60m },
                new ServiceItem { Name = "X-Quang tay/chân", Price = 40m },
            };
            categories["Vắc-xin"] = new List<ServiceItem>
            {
                new ServiceItem { Name = "Vắc-xin cúm", Price = 25m },
                new ServiceItem { Name = "Vắc-xin phòng bệnh", Price = 45m },
            };
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = cboCategory.SelectedItem as string;
            if (key == null) return;
            availableSource.DataSource = new BindingList<ServiceItem>(categories[key]);
        }

        private void AddSelected(ServiceItem item)
        {
            if (item == null) return;
            var list = selectedSource.DataSource as BindingList<ServiceItem>;
            if (list == null)
            {
                list = new BindingList<ServiceItem>();
                selectedSource.DataSource = list;
            }
            // avoid duplicates
            if (!list.Any(i => i.Name == item.Name && i.Price == item.Price))
            {
                list.Add(item);
                RecalculateTotals();
            }
        }

        private void RemoveSelected(ServiceItem item)
        {
            if (item == null) return;
            var list = selectedSource.DataSource as BindingList<ServiceItem>;
            if (list == null) return;
            var existing = list.FirstOrDefault(i => i.Name == item.Name && i.Price == item.Price);
            if (existing != null)
            {
                list.Remove(existing);
                RecalculateTotals();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var item = lstAvailableServices.SelectedItem as ServiceItem;
            AddSelected(item);
        }

        private void lstAvailableServices_DoubleClick(object sender, EventArgs e)
        {
            var item = lstAvailableServices.SelectedItem as ServiceItem;
            AddSelected(item);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var item = lstSelectedServices.SelectedItem as ServiceItem;
            RemoveSelected(item);
        }

        private void lstSelectedServices_DoubleClick(object sender, EventArgs e)
        {
            var item = lstSelectedServices.SelectedItem as ServiceItem;
            RemoveSelected(item);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            selectedSource.DataSource = new BindingList<ServiceItem>();
            RecalculateTotals();
        }

        private void nudDiscount_ValueChanged(object sender, EventArgs e)
        {
            RecalculateTotals();
        }

        private void RecalculateTotals()
        {
            var list = selectedSource.DataSource as BindingList<ServiceItem>;
            decimal total = 0m;
            if (list != null) total = list.Sum(i => i.Price);
            txtTotal.Text = total.ToString("C");
            decimal discountPercent = (decimal)nudDiscount.Value;
            decimal final = total * (1 - discountPercent / 100m);
            txtFinal.Text = final.ToString("C");
        }
    }
}
