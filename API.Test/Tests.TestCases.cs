using API.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Test
{
    public partial class Tests
    {
        public static readonly ReceiptPointValueWrapper[] TestCases =  //points calculated by hand 
        [
            new()
            {
                Receipt = new()
                {
                    Retailer = "Circuit City", //11
                    PurchaseDate = new(1999, 11, 24), //0
                    PurchaseTime = new(14, 45), //10
                    Items = //0
                    [
                        new() { ShortDescription = "Tomb Raider III: The Lost Artifact", Price = 49.99 }, //0
                    ]
                },
                Points = 21
            },
            new()
            {
                Receipt = new()
                {
                    Retailer = "Best Buy", //7
                    PurchaseDate = new(2011, 11, 11), //6
                    PurchaseTime = new(13, 45), //0
                    Items = //5 for 1 pair 
                    [
                        new() { ShortDescription = "Xbox 360", Price = 399.99 }, //0
                        new() { ShortDescription = "The Elder Scrolls IV: Oblivion", Price = 59.99 }, //12
                    ]
                },
                Points = 30
            },
            new()
            {
                Receipt = new()
                {
                    Retailer = "Gamestop", //8
                    PurchaseDate = DateTime.MaxValue, //6
                    PurchaseTime = new(14, 0), //0
                    Items = //0
                    [
                        new() { ShortDescription = "The Elder Scrolls VI: ?", Price = 59.99 } //0
                    ]
                },
                Points = 14
            }
            //new()
            //{
            //    Receipt = new()
            //    {
            //        Retailer = "Christmas Tree Shop 99%",
            //        PurchaseDate = new(2000, 12, 15),
            //        PurchaseTime = new(15, 11),
            //        Items =
            //        [
            //            new() { ShortDescription = "Christmas Tree", Price = 101.01 },
            //            new() { ShortDescription = "Ornaments", Price = 39.99 },
            //            new() { ShortDescription = "Star", Price = 11.75 },
            //            new() { ShortDescription = "Lights", Price = 19.99 },
            //            new() { ShortDescription = "Wrapping Paper", Price = 10.99 }
            //        ]
            //    },
            //    Points = 0
            //},
            //new()
            //{
            //    Receipt = new()
            //    {
            //        Retailer = ">BlockBuster<",
            //        PurchaseDate = new(2007, 6, 9),
            //        PurchaseTime = new(11, 19),
            //        Items =
            //        [
            //            new() { ShortDescription = "Back to the Future", Price = 18.99 },
            //            new() { ShortDescription = "Men in Black", Price = 16.25 },
            //            new() { ShortDescription = "Star Wars V: The Empire Strikes Back", Price = 20.00 },
            //            new() { ShortDescription = "Fantasia 2000", Price = 11.99 }
            //        ]
            //    },
            //    Points = 0
            //},
            //new()
            //{
            //    Receipt = new()
            //    {
            //        Retailer = "Newbury_Comics",
            //        PurchaseDate = new(2022, 2, 18),
            //        PurchaseTime = new(18, 8),
            //        Items = 
            //        [
            //            new() { ShortDescription = "Endtroducing.....", Price = 16.99 },
            //            new() { ShortDescription = "Taking up your Precious Time", Price = 10.00 },
            //            new() { ShortDescription = "Past is Prologue", Price = 21.99 },
            //            new() { ShortDescription = "Carrie and Lowell", Price = 18.99 },
            //            new() { ShortDescription = "Esja", Price = 20.99 }
            //        ]
            //    },
            //    Points = 0
            //}
        ];
    }
}
