
-- 6. Select the orders with Amt above the average Amt for the customers who made them
SELECT o.*
FROM orders o
JOIN (
  SELECT cnum, AVG(amt) AS avg_amt
  FROM orders
  GROUP BY cnum
) avg_customer ON o.cnum = avg_customer.cnum
WHERE o.amt > avg_customer.avg_amt;

-- 7. Select the sums of all Amt for each date, which are at least $2000 over the maximum Amt for the same date
SELECT odate, SUM(amt) AS total_amt
FROM orders
GROUP BY odate
HAVING SUM(amt) >= MAX(amt) + 2000;

-- 8. Select the orders with Amt above the average Amt for all orders. Display cname, onum, amt
SELECT c.cname, o.onum, o.amt
FROM orders o
JOIN customers c ON o.cnum = c.cnum
WHERE o.amt > (SELECT AVG(amt) FROM orders);

-- 9. Select the orders with Amt above the average Amt for orders done on the same date. Display cname, onum, odate, amt
SELECT c.cname, o.onum, o.odate, o.amt
FROM orders o
JOIN customers c ON o.cnum = c.cnum
WHERE o.amt > (
  SELECT AVG(amt) FROM orders WHERE odate = o.odate
);

-- 10. Count the number of the orders done on each date with Amt above the average Amt for all orders. Display odate, number_of_orders
SELECT odate, COUNT(*) AS number_of_orders
FROM orders
WHERE amt > (SELECT AVG(amt) FROM orders)
GROUP BY odate;
