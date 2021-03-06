﻿using PayAway.WebAPI.Entities.v1;
using PayAway.WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayAway.WebAPI.Entities.Database
{
    public class OrderDBE
    {
        /// <summary>
        /// DB Generated PK for this Order.
        /// </summary>
        /// <value>The order identifier.</value>
        [Key]
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Publicly Usable Unique Identifier for this Order
        /// </summary>
        /// <value>The order unique identifier.</value>
        [Required]
        public Guid OrderGuid { get; set; }

        public int? RefOrderId { get; set; }

        /// <summary>
        /// ID of the merchant this Order is related to
        /// </summary>
        /// <value>The merchant identifier.</value>
        [Required]
        public int MerchantId { get; set; }

        [Required]
        public DateTime OrderDateTimeUTC { get; set; }

        [Required]
        public Enums.ORDER_STATUS Status { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string CreditCardNumber { get; set; }

        public string AuthCode { get; set; }

        public decimal TipAmount { get; set; }

        public int ExpMonth { get; set; }

        public int ExpYear { get; set; }

        // Navigation Property
        public MerchantDBE Merchant { get; set; }

        // Navigation Property
        public List<OrderEventDBE> OrderEvents { get; set; }

        // Navigation Property
        public List<OrderLineItemDBE> OrderLineItems { get; set; }

        #region === Type Conversion Methods ================================
        public static explicit operator MerchantOrderMBE(OrderDBE from)
        {
            MerchantOrderMBE to = null;

            if (from != null)
            {
                to = new MerchantOrderMBE()
                {
                    OrderId = from.OrderId,
                    OrderDateTimeUTC = from.OrderDateTimeUTC,
                    OrderGuid = from.OrderGuid,
                    OrderStatus = from.Status,
                    PhoneNumber = from.PhoneNumber,
                    Name = from.CustomerName,
                    MaskedPAN = from.CreditCardNumber.Mask(),
                    AuthortizationCode = from.AuthCode,
                    TipAmount = from.TipAmount
                };
            }
            return to;
        }

        public static explicit operator OrderHeaderMBE(OrderDBE from)
        {
            OrderHeaderMBE to = null;

            if (from != null)
            {
                to = new OrderHeaderMBE()
                {
                    OrderGuid = from.OrderGuid,
                    OrderStatus = from.Status,
                    PhoneNumber = from.PhoneNumber,
                    CustomerName = from.CustomerName,
                    OrderId = from.OrderId,
                    OrderDateTimeUTC = from.OrderDateTimeUTC
                };
            }
            return to;
        }

        public static explicit operator CustomerOrderMBE(OrderDBE from)
        {
            CustomerOrderMBE to = null;

            if (from != null)
            {
                to = new CustomerOrderMBE()
                {
                    OrderGuid = from.OrderGuid,
                    OrderStatus = from.Status,
                    CustomerPhoneNo = from.PhoneNumber,
                    CustomerName = from.CustomerName,
                    OrderId = from.OrderId,
                    OrderDateTimeUTC = from.OrderDateTimeUTC,
                    MerchantName = from.Merchant.MerchantName,
                    IsSupportsTips = from.Merchant.IsSupportsTips,
                    MaskedPAN = from.CreditCardNumber.Mask(),
                    AuthortizationCode = from.AuthCode,
                    OrderSubTotal = (from.OrderLineItems != null) ? from.OrderLineItems.Sum(oli => oli.ItemUnitPrice) : 0.0M,
                    TipAmount = from.TipAmount,
                    LogoFileName = from.Merchant.LogoFileName
                };
            }
            return to;
        }
        #endregion
    }
}
