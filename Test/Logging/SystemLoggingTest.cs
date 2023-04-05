namespace Test.Logging;

public class SystemLoggingTest
{
    public class MetadataManagement
    {
        /*
         * Marking data as personal data
         * - The command that was used
         * - Number of rows affected
         * - What metadata is missing
         *
         * Updating metadata entry
         * - The command that was used
         *
         * Objections
         *
         * Revoking consent
         */
    }

    public class Vacuuming
    {
        /*
         * When vacuuming runs
         * - The rule that was enforced
         * - The query that was run
         * - Affected tables
         * - Number of affected rows
         *
         * When a vacuuming rule is added
         * When a vacuuming rule is deleted
         */
    }

    public class SchemaEvolution
    {
        /*
         * When a database starts to be managed
         *
         * When a change is made to the schema
         * - The command that was run
         * - Graph of the new schema
         * - The changes that were made
         */
    }
}