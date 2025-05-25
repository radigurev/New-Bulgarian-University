package models;

import models.enums.PaperSize;
import models.enums.PrintType;

import java.io.Serializable;
import java.math.BigDecimal;
public class Publication implements Serializable {
    private String title;
    private int totalCopies;
    private int pagesPerCopy;
    private BigDecimal unitPrice;
    private PrintType mode;

    public Publication(String title,
                       int totalCopies,
                       int pagesPerCopy,
                       BigDecimal unitPrice,
                       PrintType mode) {
        this.title = title;
        this.totalCopies = totalCopies;
        this.pagesPerCopy = pagesPerCopy;
        this.unitPrice = unitPrice;
        this.mode = mode;
    }

    // --- Getters & setters only ---
    public String getTitle() { return title; }
    public void setTitle(String title) { this.title = title; }

    public int getTotalCopies() { return totalCopies; }
    public void setTotalCopies(int totalCopies) { this.totalCopies = totalCopies; }

    public int getPagesPerCopy() { return pagesPerCopy; }
    public void setPagesPerCopy(int pagesPerCopy) { this.pagesPerCopy = pagesPerCopy; }

    public BigDecimal getUnitPrice() { return unitPrice; }
    public void setUnitPrice(BigDecimal unitPrice) { this.unitPrice = unitPrice; }

    public PrintType getMode() { return mode; }
    public void setMode(PrintType mode) { this.mode = mode; }

    /**
     * Calculates revenue (applies discount if quantity exceeds threshold).
     */
    public BigDecimal totalRevenue(BigDecimal discountRate, int discountThreshold) {
        BigDecimal gross = unitPrice.multiply(BigDecimal.valueOf(totalCopies));
        if (totalCopies > discountThreshold) {
            return gross.multiply(BigDecimal.ONE.subtract(discountRate));
        }
        return gross;
    }
}

