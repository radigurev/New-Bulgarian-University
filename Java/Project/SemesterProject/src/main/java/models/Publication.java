package models;

import models.base.PrintMode;
import models.enums.PaperSize;

import java.math.BigDecimal;

public class Publication {
    private String title;
    private int totalCopies;
    private BigDecimal unitPrice;

    public Publication(String title, int totalCopies, BigDecimal unitPrice) {
        this.title = title;
        this.totalCopies = totalCopies;
        this.unitPrice = unitPrice;
    }

    public String getTitle() { return title; }
    public int getTotalCopies() { return totalCopies; }
    public BigDecimal getUnitPrice() { return unitPrice; }

    public BigDecimal totalRevenue(BigDecimal discountRate, int discountThreshold) {
        BigDecimal gross = unitPrice.multiply(BigDecimal.valueOf(totalCopies));
        if (totalCopies > discountThreshold) {
            return gross.multiply(BigDecimal.ONE.subtract(discountRate));
        }
        return gross;
    }
}
