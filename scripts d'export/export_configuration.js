/**
 * Export configuration script template.
 * 
 * This script is included in the 'Export Script'.
 * You can put here global variables and functions that you want to be available in the 'Export Script'.
 * 
 * This file also includes the basic functions that are global in nature 
 * and are vital for configuring an Export Destination.
 * You may either leave these methods in this template empty or remove them,
 * if you don't want to handle the specific events.
 */
importPackage(java.io);
importPackage(java.lang);

log.debug("test debug export configuration");
log.info("test info export configuration");
log.warn("test warn export configuration");
log.error("test error export configuration");

out.println("test out.println script export configuration");

/**
* This method is called to configure or reconfigure this Export Destination. This method is called
* on a new Export Destination instance, right after it has been created, or an existing
* instance for reconfiguration, after it has been stopped.
* Implementations should acquire all their required static (independent of any specific
* item that gets exported) configuration at this time and make sure it is complete and correct.
* The 'configuration' is a reference to the actual Configuration java Object.
* The configuration object provides access to the components on the dialog, namely getParameters, getParameter
* The 'log' is a logging facility for use by this Export Destination instance
*/
function configure(configuration, log) {
  out.println("script de configuration - configure");
}

/**
* This method expresses the level of parallelism supported by this Export Destination. The Export
* Server will allow at most the reported number of threads to concurrently export items to this
* Export Destination instance. If this Export Destination only supports
* single-threaded operation, this method should return 1.
*/
function getMaxConcurrentThreads() {
  return 1;
}

/**
* This method is called to start this Export Destination instance. This method allows the
* Export Destination to perform any pre-operational activities, resource allocations,
* etc. Upon successfully returning from this method, the Export Server considers this
* Export Destination instance fully started, ready to export items.
*/
function start() {
  out.println("script de configuration - start");
}

/**
* This method is called to stop this Export Destination instance. This method signals the
* Export Destination it will not process any more items, at least not before another
* call to start(), therefore it should immediately let go of any resources it
* uses, disconnect from any external systems and so on. Upon successfully returning from this
* method, the Export Server considers this Export Destination instance fully stopped,
* ready to be either reconfigured and restarted, as if it were new, or discarded.
*/
function stop() {
  out.println("script de configuration - stop");
}