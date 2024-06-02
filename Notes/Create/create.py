import dominate
from dominate.tags import *
import dominate.tags
import get_results as done
import os

PATH = r'Versions'

USEADDITIONAL = False

TO_BE_UPDATED = ['b090rereremaze','b090reremaze2', 'b090rereremaze2', 'b090rereremaze22', 'b090rereremaze222'] # insert result to add its html file and update its graphs '078m', '078t', '078mt'
# LOCKED = ['011', '011Config', '021', '040', '061', '061Colab', '072Colab', '072_2Colab', '073', '074', '074Config1', '074Config2', '074ConfigLRate', '074ConfigUnitsLayer', '075', '076', '076maze', '076maze2', '076maze2fromrun', '076maze3fromrun', '076not_focused', '076old', '076stacktest', '076targetrun', '077', '077Config2', '077Config3', '077Config4', '077n', '077O', '078', '078colab']
LOCKED = []

def read_file(file_path):
    lines = """"""
    with open(file_path, 'r') as file:
        lines = file.read()
    return lines

def space():
    br()
    br()
    br()
    br()

def get_list(all = False, index = False):
    filtered_list = []
    filtered_locked_list = []
    if not all > 0 and len(TO_BE_UPDATED) :
        filtered_locked_list = TO_BE_UPDATED
    else:
        results_list = os.listdir(PATH)
        for item in results_list:
            if '.' not in item:
                filtered_list.append(item)
        if not index:
            for item in filtered_list:
                if item not in LOCKED:
                    filtered_locked_list.append(item)
        else: filtered_locked_list = filtered_list
    return filtered_locked_list

def create_index():
    doc = dominate.document(title='Index')
    results_list = get_list(all=True, index = True)
    with doc.head:
        link(rel='stylesheet', href='style.css')
    
    with doc:
        h1('Versions')
        with div(id='header').add(ol()):
            for item in results_list:
                li(a(item, href='versions/%s.html' % item))
    with open(f'html/index.html', 'w+', encoding='utf-8') as f:
        f.write(str(doc))

def create_template(version):
    toggle = 1
    doc = dominate.document(title=version)
    
    with doc.head:
        dominate.tags.link(rel='stylesheet', href='..\style.css')

    with doc.body:
        with div(cls='top-left'):
            a("Go to Top", onclick="scrollToTop()")
            a("Shrunk", onclick="shrunk()")
            a('Save', onclick="readCurrentHTMLFileAndSave()")
            a("Back to Versions", href='../index.html', id='header')
        br()
        br()
        br()
        h1(a(version, onclick=f"toggleAccordion(this, {toggle})", cls='content-header'))
        toggle+=1
        with div(cls="accordion-content").add(ol()):
            scripts_folder = f"Versions\{version}\Files\Scripts"
            how_works = os.path.isdir(scripts_folder)
            li(a("Notes", href='#notes', id='OLnotes'))
            li(a("Reward", href='#reward'), id='OLreward')
            if how_works:
                li(a("How Works", href='#howworks'), id='OLhowworks')
            li(a("Graphs", href='#graphs'), id='OLgraphs')
            if USEADDITIONAL:
                li(a("Additional", href='#additional'), id='OLadditional')
            
        b(h2(a('Notes',id='notes')),onclick=f"toggleAccordion(this, {toggle})", cls='content-header')
        toggle+=1
        
        with div(id='textnotes',cls="accordion-content"):
            textarea(id='textAreaNotes', placeholder='Type or insert text here...', rows=5, cols=50)
            button('Insert Text', onclick="createParagraph('textnotes', 'textAreaNotes')")
        
        b(h2(a('Principle & Reward',onclick=f"toggleAccordion(this, {toggle})", cls='content-header'), id='reward'))
        toggle+=1
        
        with div(id='textreward',cls="accordion-content"):
            textarea(id='textAreaReward', placeholder='Type or insert text here...', rows=5, cols=50)
            button('Insert Text', onclick="createParagraph('textreward', 'textAreaReward')")

        if USEADDITIONAL:
            b(h2(a('Additional'), id='additional'),onclick=f"toggleAccordion(this, {toggle})", cls='content-header')
            toggle+=1
            
            with div(id='textadditional',cls="accordion-content"):
                textarea(id='textAreaAdditional', placeholder='Type or insert text here...', rows=5, cols=50)
                button('Insert Text', onclick="createParagraph('textadditional', 'textAreaAdditional')")

        if how_works:
            b(h2(a('How Works', onclick=f"toggleAccordion(this, {toggle})", cls='content-header'), id='howworks'))
            toggle+=1
            
            scripts = os.listdir(scripts_folder)
            with div(cls="accordion-content"):
                with div().add(ul()):
                    for script in scripts:
                        path = "..\..\\"+scripts_folder+"\\"+script
                        li(a(script, href=path, target='_blank'), p('show code', onclick=f"toggleAccordion(this, {toggle})", cls='underline', style='margin:0px 10px;color:#919191;'), style='display:flex;justify-content: flex-start;')
                        toggle+=1
                        file = read_file(scripts_folder+"\\"+script)
                        textarea(file, rows=20, cols=120, readonly=True, cls="accordion-content")

        b(h2(a('Graphs'), id='graphs'), cls='content-header')

        with div():
            subfolders = ["Environment", "Losses", "Policy", "Custom"]
            for folder in subfolders:
                if(os.path.isdir(f"Versions\{version}\{folder}")):
                    links = os.listdir(f"Versions\{version}\{folder}")
                    links = [filename for filename in links if filename.endswith(".png")]
                    h3(folder+":")
                    with div(style='display: flex;flex-wrap: wrap;gap: .1px;width: 100%;align-items: center;text-align: center;'):
                        for link in links:
                            with div(style='width: 26%;padding: 0px;', cls='tables'):
                                image_name = ""
                                second_upper = 0
                                for word in link:
                                    if word != '_':
                                        if word.isupper():
                                            if second_upper>0:
                                                image_name+=' '
                                            second_upper+=1
                                        image_name+=word
                                    else:break
                                h4(image_name, style='margin: 0px 0px 12px 0px;')
                                img(src=f"..\..\Versions\{version}\{folder}\\"+link, width='100%', style='margin: 0;border:0;')
        dominate.tags.script(type='text/javascript', src='..\script.js')

    return doc  

def create_versions(all = False):
    for item in get_list(all):
        with open(f'html/versions/{item}.html', 'w+', encoding='utf-8') as f:
            f.write(str(create_template(item)))

def addHTML():
    create_index()
    create_versions()

# Start from start
def createOrUpdateAll():
    done.do_the_work(get_list(all = True), getall = True)
    create_versions(all = True)

# Reset all HTML files
def updateHTMLsAll():
    create_index()
    create_versions(all=True)

# Set TO_BE_UPDATED and update graph and files of the results in TO_BE_UPDATED
def updateSpesified():
    done.do_the_work(get_list())
    addHTML()
    
# Set TO_BE_UPDATED and update ONLY HTML files of the results in TO_BE_UPDATED
def updateSpesifiedHTML():
    addHTML()


updateSpesified()
updateSpesifiedHTML()