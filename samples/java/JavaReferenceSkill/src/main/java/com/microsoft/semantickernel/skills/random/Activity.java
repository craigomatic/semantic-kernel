package com.microsoft.semantickernel.skills.random;

/**
 * This class represents an Activity with its properties.
 */
    record Activity(
            String activity,
            String type,
            int participants,
            double price,
            String link,
            String key,
            float accessibility) { }
