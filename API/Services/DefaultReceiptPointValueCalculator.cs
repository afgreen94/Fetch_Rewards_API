using API.CallResults;
using API.Contracts;
using static API.Contracts.IReceiptPointValueCalculator;
using MS = Microsoft.Extensions.Configuration;

namespace API.Services
{


    /*These rules collectively define how many points should be awarded to a receipt.

        One point for every alphanumeric character in the retailer name.
        50 points if the total is a round dollar amount with no cents.
        25 points if the total is a multiple of 0.25.
        5 points for every two items on the receipt.
        If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
        6 points if the day in the purchase date is odd.
        10 points if the time of purchase is after 2:00pm and before 4:00pm.*/

    public class DefaultReceiptPointValueCalculator : IReceiptPointValueCalculator
    {

        private readonly IConfiguration configuration;

        private readonly static ClockTime TwoPm = new(14, 0);
        private readonly static ClockTime FourPm = new(16, 0);

        public DefaultReceiptPointValueCalculator() { }
        public DefaultReceiptPointValueCalculator(IConfiguration configuration) { this.configuration = configuration; }

        public ICalculatePointsForReceiptResult CalculatePointsValueForReceipt(Receipt receipt)
        {
            try
            {
                var points = 0;

                var itemCount = 0;
                var totalPrice = 0.0;

                foreach (var item in receipt.Items)
                {
                    //count items 
                    itemCount++;
                    //total price 
                    totalPrice += item.Price;

                    //item description rule 
                    if (item.ShortDescription.Length % 3 == 0)
                        points += (int)Math.Ceiling(0.2 * item.Price);
                }

                if (totalPrice % 1 == 0)
                    points += 75; //75 points for round dollar + multiple of 0.25
                else if (totalPrice % 0.25 == 0)
                    points += 25; //else 25 points if multiple of 0.25

                //5 points per 2 items on list 
                points += 5 * (int)Math.Floor((decimal)itemCount / 2);

                //1 point per alphanumeric in retailer name
                points += receipt.Retailer.ToCharArray().Select(c => char.IsLetterOrDigit(c) ? 1 : 0).Sum();

                //6 points for odd purchase day 
                points += receipt.PurchaseDate.Day % 2 == 1 ? 6 : 0;

                //10 points if purchase between 2pm and 4pm, non inclusive 
                points += receipt.PurchaseTime > TwoPm && 
                          receipt.PurchaseTime < FourPm 
                          ? 10 : 0;

                return new CalculatePointsForReceiptResult(points);
            }
            catch (Exception ex) { return new CalculatePointsForReceiptResult(true, ex.Message); }
        }

        public class CalculatePointsForReceiptResult : CallResult<int>, ICalculatePointsForReceiptResult
        {
            public CalculatePointsForReceiptResult(int value) : base(value) { }
            public CalculatePointsForReceiptResult(bool failed, string errorText) : base(failed, errorText) { }
        }


        //for calculator settings
        public interface IConfiguration { }
        public class Configuration(MS.IConfiguration configuration) : MicrosoftConfigurationWrapper(configuration), IConfiguration { }
    }
}
