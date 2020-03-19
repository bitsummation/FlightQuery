Ability to query the flighaware xml api using sql syntax. One simple example below.
```sql
select aircrafttype, actual_ident, ident, seats_cabin_business
from AirlineFlightSchedules
where departuretime > '2020-1-21 9:15' and origin = 'KATL' and destination = 'KEWR' and ident = 'DAL503'
```
More examples to follow with joins and inner queries.
