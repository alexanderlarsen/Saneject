#!/usr/bin/env python3
"""
fix-api-toc.py
Strips the 'Plugins.Saneject.' prefix from namespace names
in api/toc.yml and nests namespaces hierarchically by dot-segment.

Run after `docfx metadata` (or after a full `docfx` build):
    python fix-api-toc.py
"""
import re, os

PREFIX = "Plugins.Saneject."
TOC_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "api", "toc.yml")

# ── Parse ─────────────────────────────────────────────────────────────────────

NS_PAT = re.compile(
    r'^- uid: (.+)\n  name: (.+)\n  type: Namespace\n((?:  .+\n)*)',
    re.MULTILINE
)
ITEM_PAT = re.compile(
    r'  - uid: (.+)\n    name: (.+)\n    type: (.+)'
)

def parse(content):
    namespaces = []
    for m in NS_PAT.finditer(content):
        uid, name, body = m.group(1), m.group(2), m.group(3)
        classes = [
            {'uid': im.group(1), 'name': im.group(2), 'type': im.group(3)}
            for im in ITEM_PAT.finditer(body)
        ]
        namespaces.append({'uid': uid, 'name': name, 'classes': classes})
    return namespaces

# ── Build tree ────────────────────────────────────────────────────────────────

def build_tree(namespaces):
    root = {}
    for ns in namespaces:
        short = ns['name'][len(PREFIX):] if ns['name'].startswith(PREFIX) else ns['name']
        parts = short.split('.')
        node = root
        for p in parts[:-1]:
            node.setdefault(p, {'uid': None, 'classes': [], 'children': {}})
            node = node[p]['children']
        node.setdefault(parts[-1], {'uid': None, 'classes': [], 'children': {}})
        node[parts[-1]]['uid'] = ns['uid']
        node[parts[-1]]['classes'] = ns['classes']
    return root

# ── Emit ──────────────────────────────────────────────────────────────────────

def emit(tree, level=1):
    out = []
    p = '  ' * level
    for seg in sorted(tree):
        node = tree[seg]
        out.append(f"{p}- name: {seg}")
        if node['uid']:
            out.append(f"{p}  uid: {node['uid']}")
            out.append(f"{p}  type: Namespace")
        if node['classes'] or node['children']:
            out.append(f"{p}  items:")
            for cls in node['classes']:
                cp = '  ' * (level + 1)
                out.append(f"{cp}- uid: {cls['uid']}")
                out.append(f"{cp}  name: {cls['name']}")
                out.append(f"{cp}  type: {cls['type']}")
            if node['children']:
                out.extend(emit(node['children'], level + 1))
    return out

# ── Main ──────────────────────────────────────────────────────────────────────

def main():
    with open(TOC_PATH, encoding='utf-8') as f:
        content = f.read()

    ml_match = re.search(r'memberLayout:\s*(\S+)', content)
    member_layout = ml_match.group(1) if ml_match else 'SamePage'

    namespaces = parse(content)
    print(f"Found {len(namespaces)} namespaces")

    tree = build_tree(namespaces)
    lines = ["### YamlMime:TableOfContent", "items:"]
    lines.extend(emit(tree))
    lines.append(f"memberLayout: {member_layout}")

    with open(TOC_PATH, 'w', encoding='utf-8', newline='\n') as f:
        f.write('\n'.join(lines) + '\n')

    print(f"Rewrote {TOC_PATH}")

if __name__ == '__main__':
    main()
