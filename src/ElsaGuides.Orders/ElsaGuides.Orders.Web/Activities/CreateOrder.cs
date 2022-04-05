using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using ElsaGuides.Orders.Web.Contracts;
using ElsaGuides.Orders.Web.Models.Entities;

namespace ElsaGuides.Orders.Web.Activities;

[Activity(
    Category = "Orders",
    Description = "Create a new order for the specified customer.",
    Outcomes = new[] { OutcomeNames.Done })]
public class CreateOrder : Activity
{
    private readonly IOrderService _orderService;

    public CreateOrder(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [ActivityInput(
        Label = "Customer ID",
        Hint = "The customer ID for which to create an order.",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string CustomerId { get; set; } = default!;

    [ActivityInput(
        Label = "Description",
        Hint = "The description of the order.",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
    public string OrderDescription { get; set; } = default!;

    [ActivityOutput] public Order? Output { get; set; }

    protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
    {
        var order = await _orderService.CreateOrderAsync(CustomerId, OrderDescription, context.CancellationToken);
        Output = order;
        return Done();
    }
}