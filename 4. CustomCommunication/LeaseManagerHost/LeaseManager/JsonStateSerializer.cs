using System.Fabric.Replication;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace LeaseManager
{
    public class JsonStateSerializer<T> : IStateSerializer<T> {
        public T Read(BinaryReader binaryReader)
        {
            var jsonSerializer = new JsonSerializer();
            var bsonReader = new BsonReader(binaryReader);
            return jsonSerializer.Deserialize<T>(bsonReader);
        }

        public void Write(T value, BinaryWriter binaryWriter)
        {
            var jsonSerializer = new JsonSerializer();
            var bsonWriter = new BsonWriter(binaryWriter);
            jsonSerializer.Serialize(bsonWriter, value);
        }

        public T Read(T currentValue, BinaryReader binaryReader)
        {
            return this.Read(binaryReader);
        }

        public void Write(T currentValue, T newValue, BinaryWriter binaryWriter)
        {
            this.Write(newValue, binaryWriter);
        }
    }
}