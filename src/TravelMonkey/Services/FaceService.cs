using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using TravelMonkey.Models;

namespace TravelMonkey.Services
{
    public class FaceService
    {
        private readonly FaceClient _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(ApiKeys.ComputerVisionApiKey))
        {
            Endpoint = ApiKeys.ComputerVisionEndpoint
        };

        public async Task<string> AddPicture(Stream pictureStream)
        {
            try
            {
                IList<FaceAttributeType> faceAttributes =
                    new FaceAttributeType[]
                    {
                        FaceAttributeType.Emotion
                    };

                IList<DetectedFace> faceList =
                    await _faceClient.Face.DetectWithStreamAsync(
                        pictureStream, true, false, faceAttributes);
                if (faceList.Any())
                {
                    var description = GetFaceDescription(faceList.First());
                    return $"This places seems to make you feel {description.ToLower()}";
                }
                else
                {
                    return string.Empty;
                }
                
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        public string GetFaceDescription(DetectedFace face)
        {
            Emotion emotionScores = face.FaceAttributes.Emotion;

            var emotionProperties = face.FaceAttributes.Emotion.GetType().GetProperties();
            (string Emotion, double Value) highestEmotion = ("Anger", face.FaceAttributes.Emotion.Anger);
            foreach (var e in emotionProperties)
            {
                if (((double)e.GetValue(face.FaceAttributes.Emotion, null)) > highestEmotion.Value)
                {
                    highestEmotion.Emotion = e.Name;
                    highestEmotion.Value = (double)e.GetValue(face.FaceAttributes.Emotion, null);
                }
            }

            return highestEmotion.Emotion;
        }
    }
}
