﻿using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App5.AddItems
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Assault_rifles : ContentPage
    {
        public List<Item> Items { get; set; }
        public Player Player { get; set; }
        public string room;

        public Assault_rifles (Player player, string _room)
        {
            Player = player;
            room = _room;

            InventoryItems listItem = new InventoryItems();

            ItemReq req = new ItemReq() { type = "weapons", subtype = "Автомат" };

            string post = JsonConvert.SerializeObject(req);
            listItem = JsonConvert.DeserializeObject<InventoryItems>(Methods.POST_request(post, "get-item-list-by-type"));

            Items = new List<Item>();

            foreach (Item item in listItem.item)
            {
                Items.Add(new Item { id=item.id, name = item.name, caliber = item.caliber, cost = item.cost, country = item.country, accuracy = item.accuracy, damage = item.damage, type = item.type });
            }

            this.BindingContext = this;

            InitializeComponent();
        }

        private async void Item_Clicked(object source, ItemTappedEventArgs e)
        {
            Item selectedItem = e.Item as Item;
            if (selectedItem != null)
            {
                string insert = ";" + selectedItem.id;
                if (String.Equals(Player.weapon_ids, null) || String.Equals(Player.weapon_ids, "")) Player.weapon_ids = selectedItem.id;
                else Player.weapon_ids = Player.weapon_ids.Insert(Player.weapon_ids.Length, insert);
                UserDialogs.Instance.ShowLoading("Добавляем...");
                await RefreshPlayer();
                UserDialogs.Instance.HideLoading();
            }
        }


        private Task RefreshPlayer()
        {
            return Task.Run(() =>
            {
                string serialized = JsonConvert.SerializeObject(Player);
                Methods.POST_request(serialized, "update-user-info", room);
            });
        }
    }
}