package models;

import exceptions.NoPaperException;
import exceptions.UnsupportedPrintModeException;

import java.util.HashMap;
import java.util.Map;

public class PrintingMachine {
    private String id;
    private int loadedSheets;

    public PrintingMachine(String id) {
        this.id = id;
    }
    public String getId() { return id; }
    public int getLoadedSheets() { return loadedSheets; }
    public void loadPaper(int sheets) { loadedSheets += sheets; }

    public void print(Publication pub, int copies)
            throws UnsupportedPrintModeException, NoPaperException {
        //TODO
    }
//        if (pub.getMode() == PrintMode.COLOR && capability == PrintMode.BLACK_WHITE) {
//            throw new UnsupportedPrintModeException("Machine cannot print in color");
//        }
//        int pagesNeeded = pub.getPagesPerCopy() * copies;
//        if (pagesNeeded > loadedSheets) {
//            throw new NoPaperException("Not enough paper loaded");
//        }
//        loadedSheets -= pagesNeeded;
//        printed.merge(pub, copies, Integer::sum);
//    }

}
