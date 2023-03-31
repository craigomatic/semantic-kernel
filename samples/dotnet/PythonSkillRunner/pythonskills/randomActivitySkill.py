import requests
import json
class RandomActivitySkill:

    def getRandomActivity():
        result = requests.get('https://www.boredapi.com/api/activity').text;
        activityObject = json.loads(result);
        return activityObject