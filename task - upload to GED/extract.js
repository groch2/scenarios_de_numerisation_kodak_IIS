/**
 * This function is called whenever extraction is complete for an index field.
 * 
 * Note that it is possible to define more than one index zones for the same
 * index field. This function is called once, when the extraction for all related index
 * zones is complete. The function provides a list that contains extracted data
 * from all index zones. As a convenience it also provides the extracted
 * data with the highest confidence value.
 * 
 * The default behavior is to assign as the index field value, the extraction result 
 * with the maximum confidence. In addition, the review flag for the index field is set, 
 * if the maximum confidence is lower than the minimum confidence for the index zone. 
 * The confidence of the extraction result is saved as a property of the document, with name 
 * "sys.extraction.<index_field_name>.confidence".
 * 
 * When this function is called, the default behavior has already been applied and 
 * the index field has been filled. You may customize the behavior of the extraction
 * by using the appropriate method on the document or index field.
 * 
 * Parameters:
 *   document: the document that the extraction was performed on.
 *   field : the index field for which the extraction was performed.
 *   extractionData : a list of all the extracted data (ExtractionData objects), 
 *     from all index zones that refer to the given index field. 
 *     The ExtractionData object has the following properties:
 *       rect, value, confidence, indexZone, page.
 *   maxConfidenceExtractionData: the extracted data (Extraction object) with the 
 *     highest confidence. If the index field uses only one index zone, then the 
 *     maxConfidenceData will be equal to the only item in the extractionData list.
 */
function fieldExtractionCompleted(document, field, extractionData, maxConfidenceExtractionData) {
  debug.print("fieldExtractionCompleted - test log");
}

/**
 * This function is called whenever extraction fails for an index field and the
 * continue on error checkbox is unchecked.
 *
 * The function is called for each index zone defined for the index field.
 *
 * You may abort the rest of the extraction process by returning the value
 *  return Const.AbortOp;
 *
 * This function can be used in order to create more complex extraction abortion
 * logic, for example aborting only after X errors or if more than 50% of the
 * extraction operations failed.
 *
 * Parameters:
 *   batch: the batch that the extraction was performed on.
 *   document: the document that the extraction was performed on.
 *   field : the index field for which the extraction was performed. Can be null if no field is assigned to the zone.
 *   zone: the field index zone for which the extraction was performed.
 *   error: the error message (as string) that occurred during the extraction attempt.
 */
function onExtractionError(batch, document, field, zone, error) {
  debug.print("onExtractionError - test log");
  return Const.AbortOp;
}
