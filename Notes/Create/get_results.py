import os

RESULTS = r"C:\Users\orhun\OneDrive\Belgeler\GitHub\Dynamic-Table\results" 
PATH = r'Versions'
PORT = 6006


def get_dict(to_be_updated, get_all):
    b = ["AgentBehavior","Dynamic+Table"]
    results_list = to_be_updated
    if get_all:
        results_list = os.listdir(RESULTS)
    results = {}
    for item in results_list:
        # if '7' in item: results[item] = b[1]
        # else: results[item] = b[0]
        results[item] = b[1]
    return results
    
def link_creater(to_be_updated, get_all):
    tags = {
        "Environment":["Cumulative+Reward","Episode+Length"],
        "Losses": ["Policy+Loss","Value+Loss"],
        "Policy": ["Beta","Entropy","Epsilon","Extrinsic+Reward","Extrinsic+Value+Estimate","Learning+Rate"],
        "Custom": ["Avg+Step","Win+Percentage+of+Last+100+Episodes","Completed+Episodes","Maze+Number","Checkpoints"]
        }
    links = {}
    RunIDs = get_dict(to_be_updated, get_all)
    for id in RunIDs:
        for tag in tags:
            for innerTag in tags[tag]:
                link = f"http://localhost:{PORT}/experiment/defaultExperimentId/data/plugin/scalars/scalars?tag={tag}%2F{innerTag}&run={id}%5C{RunIDs[id]}&format=csv"
                key = f"{innerTag.replace('+','')}__{id}[{tag}]"
                links[key] = [link,f"{id}\\{tag}", f"{tag}/{innerTag}"]
    return links

def download_csv(to_be_updated, get_all):
    with open(PATH+"\log.txt", "w+") as f:
        f.close()
    import requests
    import re
    links = link_creater(to_be_updated, get_all)
    for link in links:
        url = links[link][0]
        destination_file_path = links[link][1]
        file_name = link
        error = links[link][2]
        destination_file_path = PATH+"\\"+destination_file_path
        file_name = re.sub(r'\[[^\]]*\]', '', file_name)
        response = requests.get(url)
        if response.status_code == 200:
            if not os.path.exists(destination_file_path):
                os.makedirs(destination_file_path, exist_ok=True)
            with open(destination_file_path+"\\"+file_name+".csv", 'wb') as f:
                f.write(response.content)
        else:
            bool = '7' not in url and "Custom" in url
            if not bool:
                print(f"Failed to download file -> {file_name}: {url}")
                with open(PATH+"\log.txt", "a") as f:
                    f.write(f"ERROR: {error}\n  {file_name}: {url}  PATH -> ({destination_file_path})\n")    

def smooth(scalars: list[float], weight: float) -> list[float]:  # Weight between 0 and 1
    last = scalars[0]  # First value in the plot (first timestep)
    smoothed = list()
    for point in scalars:
        smoothed_val = last * weight + (1 - weight) * point  # Calculate smoothed value
        smoothed.append(smoothed_val)                        # Save it
        last = smoothed_val
    return smoothed

def create_graphs(to_be_updated, get_all):
    import pandas as pd
    from matplotlib import pyplot as plt
    import re
    links = link_creater(to_be_updated, get_all)
    for link in links:
        try:
            destination_file_path = links[link][1]
            file_name = link
            file_name = re.sub(r'\[[^\]]*\]', '', file_name)
            destination_file_path = PATH+"\\"+destination_file_path
            columns = ["Step", "Value"]
            df = pd.read_csv(destination_file_path+"\\"+file_name+".csv", usecols=columns)
            plt.plot(df.Step, df.Value, '#a1a1a1', label="Train")
            plt.plot(df.Step, smooth(df.Value, .99), label="Train Smooth")
            # plt.show()
            try:
                plt.savefig(destination_file_path+"\\"+file_name+".png")
            except Exception as error:
                with open(PATH+"\log.txt", "a") as f:
                    f.write(f"ERROR: {file_name}\n  {error}: PATH -> ({destination_file_path})\n")    
            plt.clf()
        except Exception as error:
            with open(PATH+"\log.txt", "a") as f:
                f.write(f"ERROR: {file_name}\n  {error}: PATH -> ({destination_file_path})\n")    
            
def do_the_work(to_be_updated, getall=False):
    download_csv(to_be_updated, getall)
    create_graphs(to_be_updated, getall)