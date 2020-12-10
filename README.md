
# PayAway

- This solution was created as part of the 2020 FIS 1819 Innovation Lab initiative.
- The solution enables the PayAway product concept to be demoed to perspective customers (ie: merchants).
- The demo supports a fully interactive "customer" experience (via their smartphone) for 1 or more 3rd parties who will act the customer placing the order.
- Each demo involves three (3) roles:
    - Demo Setup
        - this person will configure the demo including the ability to personalize the screens with the appropriate merchant name, logos & website links
    - Merchant
        - this person will act as the merchant accepting the order over the phone using our "InStore POS Simulator"
    - Customer
        - this person (or persons) will use their Smartphone to interact with PayAway to submit their payment information
---
#### Business Proposition
PayAway is a solution that converts remote payment transactions such as over-the-phone into secure online payments. It's value proposition rests on 2 main pillars:
- To reduce the risk associated with exchanging payment card information openly over the phone - benefits merchants and consumers
- To support a merchant's digital strategy by converting the payment portion of a phone interaction into a more secure online payment transaction, at the completion of which the consumer can be redirected to the merchants digital properties to continue the brand interaction. This opportunity for organic brand interaction would have been completely lost at the time the consumer hangs up the phone.
---
#### Target Customer Base
The solution can be leveraged by merchants of any size and in virtually any industry or vertical: doctor's offices can use it to accelerate time to revenue; professional services providers such as contractors can leverage it to capture security deposits for an appointment or to bill pre- or post-service; online retailers offering the increasingly popular web chat support services can leverage it to drive conversion prior to terminating contact with the customer; and many other examples. All merchants can tie the solution into their digital strategies and benefit from safer payments and increased brand interaction post-contact, simply by leveraging this innovative implementation of the payment flow.

* replace the need to take Credit Card Numbers over the phone with a very low friction e-Commerce like experience for the customer.
* remove the PCI compliance challenges merchants face 
* provide an opportunity to extend the interaction with the customer
* does not require any specialized equipment, investment only modest integration changes by the merchant.

---
#### Solution Architecture

![Merchant New Order Placeholder](images/arch.jpg?raw=true)

+ Demo Website
    + Enables demos to be configured (merchant name, logo & website URL).  Additional Demo "Shadow Customers" can be configured who will automatically receive the same "Smartphone Experience".
+ Merchant Website
    + Enables entry of the order, initiates sending the payment request and viewing the order status (in a real implementation this could be replaced by the merchant's in-store POS system)
+ Customer Payment Page
    + This is "Hosted Payment Page" that allows customers to submit their payment information.
+ WebAPI
    + This is a set of business services exposed by a REST WebAPI that enable the three (3) web apps above.
    + This part of the application architecture leverages:
        + Azure hosted set of Web Services
        + .NET 5 (ASP.NET Core 5)
        + Best practice REST WebAPI use of HTTP Verbs and Response Codes
        + Swagger WebAPI documentation
        + Embedded MySQL DB
        + Leverage Async/Await pattern for improved scalability
        + Integration with Twilio SMS WebAPI for outbound text messages
        + Entity Framework and LINQ to enable Database access

---
#### Team
The Team consists of a partnership between:
+ Students from the University of Cincinnati 
   * Jacob Hornung (Intern / Front End Developer)
   * Spencer Kleeh (Intern / Front End Developer)
   * Gabriel Levit (Intern / Biz Services Developer)
   * Corey Redd (Intern / Scrum Master)

+ Employees from the Merchant Solutions division of FIS/Worldpay
   * Tom Bruns (FIS Enterprise Architecture / Mentor)
   * Rich Hill (FIS Enterprise Architecture / Director, IT Architecture)
   * Marco Fernandes (FIS Enterprise Architecture / North America Commercial Effectivess and Enablement)
   * David Miller (FIS Enterprise Architecture / Mentor)
   * Kelsey Plank (FIS Enterprise Architecture / 1819 Program Manager)
