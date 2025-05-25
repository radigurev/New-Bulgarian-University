package models;

import exceptions.NoPaperException;
import exceptions.UnsupportedPrintModeException;

import java.io.Serializable;
import java.util.*;

public class PrintingMachine implements Serializable {
    private final String id;
    private int maxCapacity;
    private int loadedSheets;
    private boolean supportsColor;
    private int pagesPerMinute;
    private final List<PrintRecord> history;

    public PrintingMachine(String id,
                           int maxCapacity,
                           boolean supportsColor,
                           int pagesPerMinute) {
        this.id = id;
        this.maxCapacity = maxCapacity;
        this.supportsColor = supportsColor;
        this.pagesPerMinute = pagesPerMinute;
        this.loadedSheets = 0;

        this.history = new ArrayList<PrintRecord>();
    }

    // --- Getters & setters only ---
    public String getId() {
        return id;
    }

    public int getMaxCapacity() {
        return maxCapacity;
    }

    public void setMaxCapacity(int maxCapacity) {
        this.maxCapacity = maxCapacity;
    }

    public int getLoadedSheets() {
        return loadedSheets;
    }

    public void setLoadedSheets(int loadedSheets) {
        this.loadedSheets = loadedSheets;
    }

    public boolean isSupportsColor() {
        return supportsColor;
    }

    public void setSupportsColor(boolean supportsColor) {
        this.supportsColor = supportsColor;
    }

    public int getPagesPerMinute() {
        return pagesPerMinute;
    }

    public void setPagesPerMinute(int pagesPerMinute) {
        this.pagesPerMinute = pagesPerMinute;
    }

    /**
     * Read-only view of all print jobs.
     */
    public List<PrintRecord> getHistory() {
        return Collections.unmodifiableList(history);
    }

    /**
     * Total pages printed by this machine over its lifetime.
     */
    public int getTotalPrintedPages() {
        return history.stream()
                .mapToInt(PrintRecord::totalPages)
                .sum();
    }

    /**
     * Records a completed print job. (Service layer should perform capacity,
     * color and paper checks, then call this to record results.)
     */
    public void recordPrint(String publicationTitle,
                            int copies,
                            int pagesPerCopy,
                            boolean color) {
        history.add(new PrintRecord(publicationTitle, copies, pagesPerCopy, color));
    }
}