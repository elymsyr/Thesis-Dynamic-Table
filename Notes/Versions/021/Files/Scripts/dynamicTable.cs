    public void triggerReset(){
        lastReward = -10;
        AddReward(-10f);
    }

    private float rewardCalculate(float point)
    {
        float reward;
        if (point == 0){
            reward = (actionLimit-actionCount)/(startDistance*5)+10f;
            win++;
        }
        else{
            reward = ((startDistance-targetCloseness())*10f/startDistance) - 10f;
        }
        lastReward = reward;
        return reward;
    }    