using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SIA
{
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()
        {
            InitializeComponent();

            var masterPageItems = new List<MasterPageItem>();

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Daily Production",
                IconSource = "contacts.png",
                TargetType = typeof(DailyJOR)

            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Transfer",
                IconSource = "contacts.png",
                TargetType = typeof(TransferPage)

            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Shipment",
                IconSource = "contacts.png",
                TargetType = typeof(ShipmentPage)

            });

            //masterPageItems.Add(new MasterPageItem
            //{
            //    Title = "Sign In/Out",
            //    IconSource = "contacts.png",
            //    TargetType = typeof(Login)

            //});
            //masterPageItems.Add(new MasterPageItem
            //{
            //    Title = "Consumption",
            //    IconSource = "contacts.png",
            //    TargetType = typeof(ConsumptionPage)

            //});

            listView.ItemsSource = masterPageItems;
        }
    }
}
