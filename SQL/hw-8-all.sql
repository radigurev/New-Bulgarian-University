-- Task 1: Find how many customers are from each city (table customers)
SELECT city, COUNT(*) AS num_customers
FROM customers
GROUP BY city;

-- Task 2: Find how many customers are from each city, excluding London
SELECT city, COUNT(*) AS num_customers
FROM customers
WHERE city <> 'London'
GROUP BY city;

-- Task 3: Find how many customers are from each city, excluding San Jose
SELECT city, COUNT(*) AS num_customers
FROM customers
WHERE city <> 'San Jose'
GROUP BY city;

-- Task 3B: Find how many customers are from London, San Jose, and Berlin
SELECT city, COUNT(*) AS num_customers
FROM customers
WHERE city IN ('London', 'San Jose', 'Berlin')
GROUP BY city;

-- Task 4: List minimum and maximum order amounts for each date (table orders)
SELECT odate, MIN(amt) AS min_amt, MAX(amt) AS max_amt
FROM orders
GROUP BY odate;

-- Task 5: List snum for salespeople with more than one customer assigned
SELECT snum
FROM customers
GROUP BY snum
HAVING COUNT(*) > 1;

-- Task 6: List cnum for clients with more than one order on the same date
SELECT DISTINCT cnum
FROM orders
GROUP BY cnum, odate
HAVING COUNT(*) > 1;

-- Task 7: List sname for salespeople with more than one order with the same customer
SELECT DISTINCT sname
FROM summary
GROUP BY sname, cname
HAVING COUNT(*) > 1;

-- Task 8: Find how many salespeople are from each city, excluding London
SELECT city, COUNT(*) AS num_salespeople
FROM salespeople
WHERE city <> 'London'
GROUP BY city;

-- Task 9: List customers whose total order amount is $1,500 or more
SELECT cname
FROM summary
GROUP BY cname
HAVING SUM(amt) >= 1500;

-- Task 10: List customers whose total order amount is $500 or more, excluding orders by Serres
SELECT cname
FROM summary
WHERE sname <> 'Serres'
GROUP BY cname
HAVING SUM(amt) >= 500;

-- Task 11: List salespeople who have more than 1 order and total order amount > $2,000
SELECT sname
FROM summary
GROUP BY sname
HAVING COUNT(*) > 1
   AND SUM(amt) > 2000;

-- Task 12: List all cities in which at least two customers reside
SELECT city
FROM customers
GROUP BY city
HAVING COUNT(*) >= 2;

-- Task 13: Find average rating for customers in each city
SELECT city, AVG(rating) AS avg_rating
FROM customers
GROUP BY city;

-- Task 14: List cities where average customer rating is below 200
SELECT city
FROM customers
GROUP BY city
HAVING AVG(rating) < 200;

-- Task 15: List all dates with more than one and less than four orders
SELECT odate
FROM orders
GROUP BY odate
HAVING COUNT(*) > 1 AND COUNT(*) < 4;
