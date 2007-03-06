Module for editing Merchant Data Table.

Metadata XML must contain data like this:
<Metadata CreditInstitute="MyBank" BankCode="11222" FaxNumber="0005555-000000"/>

All properties should be supported by the choosed gateway.
Unknown properties are ignored.
Missing required properties will throw an error on checkout.

Refer to gateway/shipping documentation for a list of valid properties.

Known Limit: Even if the MarchantManager table has a Portal ID the module does changes site wide.