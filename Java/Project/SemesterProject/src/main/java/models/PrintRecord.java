package models;

import java.io.Serializable;

public class PrintRecord implements Serializable {
    private final String publicationTitle;
    private final int copies;
    private final int pagesPerCopy;
    private final boolean color;

    public PrintRecord(String publicationTitle,
                       int copies,
                       int pagesPerCopy,
                       boolean color) {
        this.publicationTitle = publicationTitle;
        this.copies = copies;
        this.pagesPerCopy = pagesPerCopy;
        this.color = color;
    }

    public String getPublicationTitle() { return publicationTitle; }
    public int getCopies()           { return copies; }
    public int getPagesPerCopy()     { return pagesPerCopy; }
    public boolean isColor()         { return color; }

    /** Total pages printed in this job. */
    public int totalPages() {
        return copies * pagesPerCopy;
    }
}