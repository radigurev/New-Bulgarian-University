use sco_db_s2025;
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
