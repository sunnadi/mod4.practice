using System;
using System.Collections.Generic;
public class Order
{
    public List<OrderItem> Items { get; private set; }
    public IPayment PaymentMethod { get; set; }
    public IDelivery DeliveryMethod { get; set; }

    public Order()
    {
        Items = new List<OrderItem>();
    }

    public void AddItem(string productName, int quantity, double price)
    {
        Items.Add(new OrderItem(productName, quantity, price));
    }

    public double CalculateTotalPrice(IDiscountCalculator discountCalculator)
    {
        double total = 0;
        foreach (var item in Items)
        {
            total += item.Quantity * item.Price;
        }
        return discountCalculator.ApplyDiscount(total);
    }

    public void ProcessPayment()
    {
        double amount = CalculateTotalPrice(new NoDiscountCalculator()); 
        PaymentMethod?.ProcessPayment(amount);
    }

    public void Deliver()
    {
        DeliveryMethod?.DeliverOrder(this);
    }
}

public class OrderItem
{
    public string ProductName { get; }
    public int Quantity { get; }
    public double Price { get; }

    public OrderItem(string productName, int quantity, double price)
    {
        ProductName = productName;
        Quantity = quantity;
        Price = price;
    }
}

public interface IPayment
{
    void ProcessPayment(double amount);
}

public interface IDelivery
{
    void DeliverOrder(Order order);
}

public interface IDiscountCalculator
{
    double ApplyDiscount(double total);
}

public class CreditCardPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Платеж на сумму {amount} обработан с помощью кредитной карты.");
    }
}

public class PayPalPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Платеж на сумму {amount} обработан через PayPal.");
    }
}

public class BankTransferPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Платеж на сумму {amount} обработан банковским переводом.");
    }
}
public class CourierDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Заказ доставлен курьером.");
    }
}
public class PostDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Заказ доставлен почтой.");
    }
}

public class PickUpPointDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Заказ готов к самовывозу.");
    }
}

public class NoDiscountCalculator : IDiscountCalculator
{
    public double ApplyDiscount(double total)
    {
        return total; 
    }
}

public class PercentageDiscountCalculator : IDiscountCalculator
{
    private readonly double _percentage;

    public PercentageDiscountCalculator(double percentage)
    {
        _percentage = percentage;
    }

    public double ApplyDiscount(double total)
    {
        return total * (1 - _percentage / 100);
    }
}
public interface INotification
{
    void SendNotification(string message);
}

public class EmailNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine("Уведомление по Email: " + message);
    }
}

public class SmsNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine("Уведомление по SMS: " + message);
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        var order = new Order();
        order.AddItem("Ноутбук", 1, 1000);
        order.AddItem("Клавиатура", 2, 50);

        order.PaymentMethod = new CreditCardPayment();
        order.DeliveryMethod = new CourierDelivery();

        double totalPrice = order.CalculateTotalPrice(new PercentageDiscountCalculator(10)); 
        Console.WriteLine($"Общая стоимость заказа с учетом скидки: {totalPrice}");
        order.ProcessPayment();

        order.Deliver();

        INotification notification = new EmailNotification();
        notification.SendNotification("Ваш заказ успешно оформлен!");
    }
}

